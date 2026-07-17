# Diseño: Confirmación de venta, Pagos/Cuotas/Caja y mejora de front

Fecha: 2026-07-17

## Alcance

Este spec cubre los puntos 3.1, 3.4, 3.5 y 3.6 del pedido original:

- **3.1** Pantalla de confirmación post-venta (resumen accionable).
- **3.4** Modelo de datos de Pagos/Cuotas/Caja.
- **3.5** Negocio (`PagoNegocio`, `CajaNegocio`) + UI de pago en `VentasFormulario.aspx` + nueva `Caja.aspx`.
- **3.6** Mejora de consistencia visual y feedback de acciones en todo el sitio.

**Fuera de alcance** (decisión explícita, no se implementan en este ciclo):

- **3.2** Email de confirmación (no se agrega SMTP a `Web.config` ni servicio de envío).
- **3.3** Descargar/compartir factura por email (no se toca `FacturaReporte.aspx`; el botón "Imprimir" que ya existe ahí no cambia).

## 3.4 — Modelo de datos

Nueva migración `Database/migrations/02_PagosCuotasCaja.sql`, siguiendo el mismo estilo que `01_Lotes.sql`: tablas + `CREATE OR ALTER PROCEDURE` para cada acceso (patrón más consistente con el módulo más reciente del proyecto que el SQL inline usado en `FacturaNegocio`).

### Tabla Pagos

```sql
CREATE TABLE [dbo].[Pagos] (
    [Id]             [int]           IDENTITY(1,1) NOT NULL,
    [IdFactura]      [int]           NOT NULL,
    [Tipo]           [varchar](20)   NOT NULL,   -- 'Efectivo' | 'Debito' | 'Credito'
    [Monto]          [decimal](12,2) NOT NULL,   -- total a pagar (incluye interés si es Credito)
    [CantidadCuotas] [int]           NOT NULL DEFAULT (0),
    [Fecha]          [datetime]      NOT NULL DEFAULT (getdate()),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pagos_Factura] FOREIGN KEY([IdFactura]) REFERENCES [dbo].[Facturas] ([Id])
)
```

### Tabla Cuotas

Solo se generan filas si `Pagos.Tipo = 'Credito'`.

```sql
CREATE TABLE [dbo].[Cuotas] (
    [Id]          [int]           IDENTITY(1,1) NOT NULL,
    [IdPago]      [int]           NOT NULL,
    [NroCuota]    [int]           NOT NULL,
    [Monto]       [decimal](10,2) NOT NULL,   -- capital de la cuota
    [Interes]     [decimal](10,2) NOT NULL,   -- interés de la cuota
    [Vencimiento] [datetime]      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cuotas_Pago] FOREIGN KEY([IdPago]) REFERENCES [dbo].[Pagos] ([Id])
)
```

### Tabla Caja

```sql
CREATE TABLE [dbo].[Caja] (
    [Id]                   [int]           IDENTITY(1,1) NOT NULL,
    [FechaApertura]        [datetime]      NOT NULL DEFAULT (getdate()),
    [MontoApertura]        [decimal](12,2) NOT NULL,
    [IdUsuarioApertura]    [int]           NOT NULL,
    [FechaCierre]          [datetime]      NULL,
    [MontoCierreDeclarado] [decimal](12,2) NULL,
    [MontoCierreCalculado] [decimal](12,2) NULL,
    [IdUsuarioCierre]      [int]           NULL,
    [Estado]               [bit]           NOT NULL DEFAULT (1),  -- 1 abierta, 0 cerrada
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Caja_UsuarioApertura] FOREIGN KEY([IdUsuarioApertura]) REFERENCES [dbo].[Usuarios] ([Id]),
    CONSTRAINT [FK_Caja_UsuarioCierre]   FOREIGN KEY([IdUsuarioCierre])   REFERENCES [dbo].[Usuarios] ([Id])
)
```

Regla de negocio: solo puede existir **una Caja abierta a la vez** (se valida en `SP_Caja_Abrir`, error si ya hay una con `Estado = 1`). La Caja **no bloquea** el registro de ventas — sirve únicamente para conciliar el efectivo cobrado en el período abierto.

### Stored procedures

- `SP_Pagos_Crear` (@IdFactura, @Tipo, @Monto, @CantidadCuotas, @NewId OUTPUT)
- `SP_Cuotas_Crear` (@IdPago, @NroCuota, @Monto, @Interes, @Vencimiento)
- `SP_Pagos_ObtenerPorFactura` (@IdFactura) — 1 fila
- `SP_Cuotas_ListarPorPago` (@IdPago)
- `SP_Caja_Abrir` (@MontoApertura, @IdUsuario, @NewId OUTPUT) — falla si ya hay una caja abierta
- `SP_Caja_ObtenerAbierta` — la caja con `Estado = 1`, si existe
- `SP_Caja_Cerrar` (@Id, @MontoCierreDeclarado, @IdUsuario) — calcula `MontoCierreCalculado = SUM(Pagos.Monto)` donde `Tipo = 'Efectivo'` y `Fecha` entre `FechaApertura` y `getdate()`, y setea `Estado = 0`, `FechaCierre = getdate()`
- `SP_Caja_Listar` — histórico de cajas (para la grilla de `Caja.aspx`)

## 3.5 — Negocio + UI de pago

### `Negocio/PagoNegocio.cs`

Mismo estilo que `LoteNegocio` (usa `AccesoDatos` + SPs, try/catch/finally con `cerrarConexion`).

- `RegistrarPago(int idFactura, decimal totalFactura, string tipoPago, int cantidadCuotas)`:
  - Inserta en `Pagos` vía `SP_Pagos_Crear`.
  - Si `tipoPago == "Credito"`: calcula interés y genera las filas de `Cuotas` vía `SP_Cuotas_Crear` (una por cuota).
  - Si `tipoPago` es `Efectivo` o `Debito`: `Monto = totalFactura`, `CantidadCuotas = 0`, sin filas en `Cuotas`.
- `ObtenerPorFactura(int idFactura)`: trae el `Pago` correspondiente + su lista de `Cuotas` (para `ConfirmacionVenta.aspx`).

**Cálculo de interés** (tasa fija mensual simple):

- Tasa configurada en `Web.config`, sección `<appSettings>` (nueva), clave `TasaInteresMensual` (ej. `"5"` = 5% mensual).
- `InteresTotal = totalFactura * (tasa / 100) * cantidadCuotas`
- `MontoTotalConInteres = totalFactura + InteresTotal`
- Cada cuota = `MontoTotalConInteres / cantidadCuotas`, repartido en partes iguales; la **última cuota absorbe la diferencia de redondeo** de centavos respecto a las anteriores.
- `Vencimiento` de la cuota N = fecha de la venta + N meses (`AddMonths(N)`).
- Cuotas permitidas: **3, 6 o 12** (combo fijo, no libre).

### `Negocio/CajaNegocio.cs`

- `AbrirCaja(decimal montoApertura, int idUsuario)` → `SP_Caja_Abrir`.
- `ObtenerCajaAbierta()` → `SP_Caja_ObtenerAbierta`.
- `CerrarCaja(int idCaja, decimal montoDeclarado, int idUsuario)` → `SP_Caja_Cerrar`.
- `Listar()` → `SP_Caja_Listar`, histórico.

`FacturaNegocio` **no se modifica**: `RegistrarVenta` sigue igual. `PagoNegocio.RegistrarPago` se invoca desde el code-behind de `VentasFormulario.aspx` después de que `RegistrarVenta` ya creó la factura, manteniendo cada clase de Negocio con una sola responsabilidad.

### `VentasFormulario.aspx` / `.aspx.cs`

Se extiende siguiendo al pie de la letra la convención ya usada en esa misma página y en `ComprasFormulario.aspx` (ambas co-escritas por el equipo): fila `div.mb-3.row.align-items-center` + `label.col-sm-2.col-form-label.fw-semibold` para el `ddlFormaPago`, agregada después del bloque "Cliente" y antes del `<hr />` de "Agregar productos"; y un bloque análogo (mismo patrón de fila) para `ddlCantidadCuotas`, que se muestra/oculta con JS simple cuando `ddlFormaPago` vale "Credito" — sin postback ni AJAX, igual que hoy no hay ningún `OnSelectedIndexChanged` con autopostback en esta página.

- `ddlFormaPago`: Efectivo / Debito / Credito (`asp:DropDownList CssClass="form-select"`, mismo control que `ddlCliente`/`ddlProducto`).
- `ddlCantidadCuotas`: 3 / 6 / 12 (`asp:DropDownList CssClass="form-select"`), envuelto en un `div` con `id` propio para el show/hide por JS.
- No hay preview en vivo del cálculo de cuotas antes de guardar (para no sumar otra ronda de postbacks); el detalle se ve recién en `ConfirmacionVenta.aspx` después de guardar.
- En `btnGuardar_Click` (mismo método existente, no uno nuevo): después de `RegistrarVenta(...)`, se llama a `new PagoNegocio().RegistrarPago(idGenerado, factura.Total, formaPago, cantidadCuotas)` dentro del mismo bloque `try/catch` que ya envuelve `RegistrarVenta`, usando `MostrarMensaje(ex.Message, false)` igual que hoy ante cualquier error. Se redirige con `Response.Redirect("ConfirmacionVenta.aspx?id=" + idGenerado, false)` (mismo patrón `Redirect(url, false)` que ya usa esta página) en vez de `FacturaReporte.aspx?id=`.

### Nueva `Caja.aspx` / `.aspx.cs`

Sigue exactamente la estructura de `VentasFormulario.aspx`/`ComprasFormulario.aspx`: `Page Title="Caja" MasterPageFile="~/Site.Master"`, header `div.d-flex.justify-content-between.align-items-center.mb-4` con `<h2>Caja</h2>` (sin link "Volver al listado" porque no hay un listado previo — el histórico está en la misma página), `div.card > card-header/card-body`, `asp:Label ID="lblMensaje" CssClass="alert d-none w-100 mb-3"` para feedback, montos ingresados con `asp:TextBox CssClass="form-control"` + el par `RequiredFieldValidator`/`RangeValidator` con `ValidationGroup` (`"AbrirCaja"` / `"CerrarCaja"`) igual que `txtCantidad`/`txtPrecioUnitario` en `ComprasFormulario`.

Acceso solo Admin/Vendedor: mismo patrón de `Ventas.aspx.cs` (`Session["Usuario"] as Usuario` + `Seguridad.SesionActiva` + `Seguridad.EsAdmin`/`EsVendedor`, redirect a `Default.aspx` si no cumple).

- Si **no hay caja abierta**: formulario de apertura (`txtMontoApertura` + validators) + `asp:Button ID="btnAbrirCaja" CssClass="btn btn-primary"`.
- Si **hay caja abierta**: muestra fecha/monto de apertura (vía `runat="server"` `<p>` igual que `lblNumeroFactura`/`lblFecha` en `FacturaReporte.aspx`, o `asp:Label`), total en efectivo acumulado hasta el momento (informativo), formulario de cierre (`txtMontoCierre` + validators) + `asp:Button ID="btnCerrarCaja" CssClass="btn btn-primary"` que al confirmar muestra el arqueo (diferencia) vía `MostrarMensaje`.
- `asp:GridView ID="dgvHistorico"` histórico de cajas cerradas (fecha apertura/cierre, montos, diferencia, usuario), `AutoGenerateColumns="false"` + `CssClass="table table-bordered table-sm"`, igual que `dgvItems` en las otras páginas.
- Codebehind: `Page_Load` con guarda `!IsPostBack` que llama a `CargarEstadoCaja()`; `btnAbrirCaja_Click`/`btnCerrarCaja_Click` con try/catch llamando a `CajaNegocio` y `MostrarMensaje(ex.Message, false)` en el catch, igual que `btnGuardar_Click` en `VentasFormulario`.

### `Site.Master` / `Site.Master.cs`

Se agrega `liCaja` dentro del dropdown "Movimientos", visible para `esAdmin || esVend` — mismo patrón que ya usan `liCompras`/`liVentas` en ese mismo grupo (no se toca la visibilidad de `liTrazabilidad`, que sigue solo Admin).

## 3.1 — Confirmación de compra

Nueva página `ConfirmacionVenta.aspx` / `.aspx.cs`, reemplaza el destino del redirect que hoy hace `VentasFormulario.btnGuardar_Click` hacia `FacturaReporte.aspx?id=`.

A diferencia de `FacturaReporte.aspx` (que es una vista de impresión standalone, con su propio `<html>`/`<head>` y sin `Site.Master` — una excepción deliberada para que `window.print()` no imprima la navbar), `ConfirmacionVenta.aspx` es una pantalla interactiva más, con navegación y botones de acción. Sigue entonces la misma estructura que `VentasFormulario.aspx`/`Ventas.aspx`, no la de `FacturaReporte.aspx`:

- `<%@ Page Title="Confirmación de venta" MasterPageFile="~/Site.Master" ... %>`, contenido en `asp:Content ContentPlaceHolderID="MainContent"`.
- Header `div.d-flex.justify-content-between.align-items-center.mb-4` con `<h2>Venta confirmada</h2>`.
- `div.card > card-header/card-body` para el resumen, mismo look que el "Datos de la venta" de `VentasFormulario`.
- Recibe `?id=<idFactura>` por querystring, mismo `int.TryParse(Request.QueryString["id"], ...)` + `MostrarError(...)` que usa `FacturaReporte.aspx.cs` ante ID inválido o ausente.
- En `Page_Load` (guardado por `!IsPostBack`) consulta `FacturaNegocio.Listar()` + `DetalleFacturaNegocio.ListarPorFactura(idFactura)` (mismo patrón que `FacturaReporte.aspx.cs`) y `PagoNegocio.ObtenerPorFactura(idFactura)`; misma validación de permiso que ya hace `FacturaReporte.aspx.cs` (`Seguridad.EsCliente(u)` + comparar `factura.Cliente.ID` contra `u.IdEntidad`, redirect a `Default.aspx` si no corresponde).
- Resumen **inline** en la misma pantalla (no requiere navegar a otra página para verlo):
  - Datos de cliente.
  - `asp:GridView AutoGenerateColumns="false" CssClass="table table-bordered table-sm"` con los items (descripción, cantidad, precio unitario, subtotal) — mismas columnas que `dgvDetalles` en `FacturaReporte.aspx`.
  - Forma de pago elegida.
  - Si es Crédito: tabla de cuotas (Nro, Monto, Interés, Vencimiento), mismo estilo de grid.
  - Total final.
- Acciones (botones `btn-outline-secondary`/`btn-primary`, mismo patrón que Cancelar/Guardar en los formularios):
  - **Ver/Imprimir factura** → link a `FacturaReporte.aspx?id=`.
  - **Nueva venta** → `VentasFormulario.aspx`.
  - **Volver al listado** → `Ventas.aspx`.

## 3.6 — Mejora de front (todo el sitio)

Dos piezas reutilizables, aplicadas de forma incremental (página por página, listado en el plan de implementación — no es un cambio masivo de una sola vez):

1. **CSS unificado**: mover los estilos `<style>` inline que hoy tiene cada `.aspx` (ej. `FacturaReporte.aspx`, y los que sumen las páginas nuevas) a `Content/Site.css` (ya existe y ya se referencia desde `Site.Master` vía el bundle `~/Content/css`), dejando en cada página solo clases Bootstrap + lo estrictamente específico de esa vista.
2. **Feedback de acciones**: helper JS chico (`Scripts/site-ui.js`, incluido desde `Site.Master` junto al script de bootstrap) con:
   - Función para mostrar un **toast** de Bootstrap 5 (éxito/error), reemplazando los `<asp:Label>` sueltos de mensaje que hoy arma cada code-behind (`MostrarMensaje` en `VentasFormulario`, `lblMensaje` en `Ventas.aspx.cs`, etc.) — se mantiene la misma lógica de "mensaje + éxito/error" en el code-behind, solo cambia el marcado que lo muestra.
   - Un **loader** simple (spinner + disable del botón) enganchado a los botones de submit vía los eventos de `PageRequestManager` (el sitio ya usa `ScriptManager`/postbacks completos, no fetch/AJAX real), para dar feedback mientras el postback resuelve.
3. Se aplica primero a las páginas nuevas (`VentasFormulario`, `ConfirmacionVenta`, `Caja`) y luego se va migrando el resto de páginas existentes reemplazando sus labels de mensaje por el toast compartido.

## Notas de implementación transversales

- Todo el código nuevo en `Negocio` sigue el patrón de `LoteNegocio` (SPs vía `AccesoDatos`, no SQL inline) — confirmado como convención de acceso a datos usada por ambos integrantes del equipo (11 de 14 clases de `Negocio` usan `setearProcedimiento`, incluyendo archivos co-escritos como `ClienteNegocio`/`ProductoNegocio`).
- Los nombres de tablas, columnas y SPs siguen la convención en español ya usada (`Pagos`, `Cuotas`, `Caja`, `SP_<Tabla>_<Accion>`).
- No se modifica `FacturaNegocio.RegistrarVenta` ni `FacturaReporte.aspx`.
- Las páginas y code-behinds nuevos (`ConfirmacionVenta.aspx`, `Caja.aspx`) y las extensiones a `VentasFormulario.aspx` siguen al pie de la letra la convención verificada en `VentasFormulario.aspx`/`ComprasFormulario.aspx`/`Ventas.aspx`/`FacturaReporte.aspx.cs` (todos co-escritos por el equipo): `Site.Master` + header con título y "Volver al listado" + `div.card`, `lblMensaje`/`MostrarMensaje`/`MostrarError` para feedback, pares `RequiredFieldValidator`+`RangeValidator` con `ValidationGroup` en inputs numéricos, `GridView AutoGenerateColumns="false"` con clases `table table-bordered table-sm`, y `Response.Redirect(url, false)`. `FacturaReporte.aspx` es la única excepción (standalone, sin `Site.Master`) por ser una vista de impresión — no se usa como modelo para las páginas nuevas.
