# Confirmación de venta, Pagos/Cuotas/Caja y mejora de front — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a post-sale confirmation screen, a Pagos/Cuotas/Caja payment model with UI in `VentasFormulario.aspx` and a new `Caja.aspx`, and unify feedback/CSS across the site — per `docs/superpowers/specs/2026-07-17-pagos-cuotas-caja-design.md`.

**Architecture:** Classic 3-layer WebForms app (`Dominio` → `Negocio` → `AplicacionWebComercio`), data access via stored procedures through the existing `AccesoDatos` wrapper (no ORM, no transactions — matches current codebase). New tables `Pagos`/`Cuotas`/`Caja` added via a numbered migration script, following the same pattern as `01_Lotes.sql`.

**Tech Stack:** ASP.NET Web Forms (.NET Framework 4.8), ADO.NET via `AccesoDatos`/`Negocio`/`Dominio` class libraries, SQL Server (stored procedures), Bootstrap 5.2.3 + jQuery 3.7 (already wired via `System.Web.Optimization`), MSTest (temporary, for the one piece of pure business logic — see Global Constraints).

## Global Constraints

- Target framework is **.NET Framework 4.8** for every project (`Dominio`, `Negocio`, `AccesoDatos`, `AplicacionWebComercio`). Classic (non-SDK) `.csproj` — new files require an explicit `<Compile Include>` / `<Content Include>` entry, they are not picked up automatically.
- The solution file is `TPC-Comercio/TPC-Comercio.slnx` (newer XML solution format — a plain `*.sln` glob will miss it, as happened during planning). Do not create a `.sln`.
- **`Dominio`, `AccesoDatos`, and `Negocio` build fine with the plain `dotnet build <csproj>` CLI** (validated live). **`AplicacionWebComercio.csproj` does NOT** — `dotnet build` fails with `MSB4019` because the bare .NET SDK has no `Microsoft.WebApplication.targets` (that only ships with Visual Studio). To build/verify it, use the full VS MSBuild against the solution:
  ```bash
  MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
  ```
  (First time only, if it errors about missing NuGet packages: run the same command with `-t:restore -p:RestorePackagesConfig=true` first — this is a one-time restore into `TPC-Comercio/packages/`, already done as of this plan's writing.) Every "build to verify it compiles" step that touches `AplicacionWebComercio.csproj` in this plan uses this exact command, not `dotnet build`.
- Data access in `Negocio` uses **stored procedures** via `AccesoDatos.setearProcedimiento(...)` (confirmed team convention — 11 of 14 existing `Negocio` classes use this, including files co-written by both teammates). Do not write inline SQL (`setearConsulta`) for new code.
- Every data-access method: `new AccesoDatos.AccesoDatos()` → `try { ... } catch (Exception ex) { throw ex; } finally { datos.cerrarConexion(); }` — copy this shape exactly, it is used in every existing `Negocio` class.
- No database transactions are used anywhere in this codebase (`FacturaNegocio.RegistrarVenta` makes multiple sequential SP calls with no transaction wrapper). Do not introduce transactions — it would be an unrequested architectural change.
- UI pages follow the convention verified in `VentasFormulario.aspx`/`ComprasFormulario.aspx`/`Ventas.aspx`/`FacturaReporte.aspx.cs` (co-written by both teammates): `Site.Master` + header (`div.d-flex.justify-content-between.align-items-center.mb-4` with `<h2>`) + `div.card > card-header/card-body`, `asp:Label id="lblMensaje" CssClass="alert d-none w-100 mb-3"` + `MostrarMensaje(msg, exito)` helper for feedback, `RequiredFieldValidator`+`RangeValidator` pairs with a shared `ValidationGroup` on numeric inputs, `GridView AutoGenerateColumns="false"` with `table table-bordered table-sm`, `Response.Redirect(url, false)`. **`FacturaReporte.aspx`/`CompraReporte.aspx` are the only exception** (standalone, no `Site.Master`, for printability) — don't use them as a markup template for new pages, only reference their code-behind data-loading pattern.
- Role checks always go through `Negocio.Seguridad` (`SesionActiva`, `EsAdmin`, `EsVendedor`, `EsCliente`, `EsProveedor`) — never compare `perfil.NombrePerfil` directly outside that class.
- `FacturaNegocio.RegistrarVenta` and `FacturaReporte.aspx` are **not modified** by this plan.
- 3.2 (email) and 3.3 (descargar/compartir factura por email) are **out of scope** — do not add SMTP config or touch the "Imprimir" button semantics on `FacturaReporte.aspx`.
- **SQL verification environment:** `AccesoDatos.cs` hardcodes its connection to `DESKTOP-K605RT2`, which is not reachable from this machine. A scratch `tpc_P3` database was created for this plan's execution on this machine's local SQL instance, reachable as `sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "..."` (Windows Authentication) — it already has the full current schema (including `Lotes`, matching `01_Lotes.sql`) loaded from `TPC_Comercio.sql`. Use this connection for every SQL verification step in this plan (Tasks 3, 5, 6) instead of `DESKTOP-K605RT2`. Do **not** change `AccesoDatos.cs`'s connection string — that stays pointed at the real dev machine for when the app is actually run there.
- **`sqlcmd -i <file>` requires a Windows backslash-style path** (`C:\path\to\file.sql`), not a forward-slash path — confirmed live: `sqlcmd` (a native Windows console tool) parses `/` inside a forward-slash path as a switch delimiter and silently truncates the argument, producing a confusing `-E and -U/-P mutually exclusive` error that has nothing to do with the actual problem. Every `sqlcmd -i` command in this plan must use backslashes.
- **Browser verification:** there is no browser-automation tool in this environment. For "manual verification in the browser" steps, start the site with IIS Express directly (`"/c/Program Files/IIS Express/iisexpress.exe" /path:"C:\UTN\TPC-Comercio\TPC-Comercio\AplicacionWebComercio" /port:59928`, run in the background) and use `WebFetch` against `http://localhost:59928/<Page>.aspx` for anything checkable by a plain GET (page loads without error, expected text/labels present, permission redirects happen for the wrong role). Steps that require an actual form POST (submitting a sale, opening/closing a caja through the UI, clicking a button) **cannot** be done this way — mark that specific part of the step as left for the human's own manual QA pass, and say so explicitly in the report rather than claiming it was verified.
- Testing strategy (explicitly agreed): there is no existing test project in this solution. A **temporary** MSTest project (`Negocio.Tests`) is added in Task 1 to TDD the one piece of genuine business logic in this feature — the interest/cuotas calculation (`PagoNegocio.CalcularPlanDeCuotas`), which is pure and DB-free. Everything that touches the database (stored procedures, `Negocio` CRUD methods) or the UI is verified manually (SQL query / browser), matching how the rest of the codebase has always been verified — this is not a gap introduced by this plan, it's the existing norm. **Task 12 deletes `Negocio.Tests`** once Task 2 is done with it.

---

## File Structure

**New files:**
- `TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj` — temporary MSTest project (deleted in Task 12).
- `TPC-Comercio/Negocio.Tests/PagoNegocioTests.cs` — TDD tests for the cuotas calculation (deleted in Task 12).
- `TPC-Comercio/Database/migrations/02_PagosCuotasCaja.sql` — tables `Pagos`/`Cuotas`/`Caja` + stored procedures.
- `TPC-Comercio/Dominio/Pago.cs`, `TPC-Comercio/Dominio/Cuota.cs`, `TPC-Comercio/Dominio/Caja.cs` — domain POCOs.
- `TPC-Comercio/Negocio/PagoNegocio.cs` — `RegistrarPago`, `CalcularPlanDeCuotas` (pure), `ObtenerPorFactura`, `ListarCuotasPorPago`, `SumarEfectivoDesde`.
- `TPC-Comercio/Negocio/CajaNegocio.cs` — `AbrirCaja`, `ObtenerCajaAbierta`, `CerrarCaja`, `Listar`.
- `TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx` / `.aspx.cs` / `.aspx.designer.cs` — post-sale confirmation screen (3.1).
- `TPC-Comercio/AplicacionWebComercio/Caja.aspx` / `.aspx.cs` / `.aspx.designer.cs` — apertura/cierre/arqueo de caja (3.5).
- `TPC-Comercio/AplicacionWebComercio/Scripts/site-ui.js` — shared toast + postback-loader helper (3.6).

**Modified files:**
- `TPC-Comercio/Dominio/Dominio.csproj` — add `<Compile Include>` for the 3 new domain classes.
- `TPC-Comercio/Negocio/Negocio.csproj` — add `<Compile Include>` for `PagoNegocio.cs`, `CajaNegocio.cs`.
- `TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj` — add `<Content>`/`<Compile>` entries for the 2 new pages and `site-ui.js`.
- `TPC-Comercio/AplicacionWebComercio/Web.config` — add `<appSettings>` with `TasaInteresMensual`.
- `TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx` / `.aspx.cs` — forma de pago UI + wiring to `PagoNegocio` + redirect target (3.5).
- `TPC-Comercio/AplicacionWebComercio/Site.Master` / `.master.cs` / `.master.designer.cs` — nav entry for Caja + `site-ui.js` include (3.5/3.6).
- `TPC-Comercio/AplicacionWebComercio/Content/Site.css` — shared toast/loader styles skip (pure JS/Bootstrap, no new CSS needed there) + scoped `.comprobante-container` rules extracted from the two print pages (3.6).
- `TPC-Comercio/AplicacionWebComercio/FacturaReporte.aspx`, `TPC-Comercio/AplicacionWebComercio/CompraReporte.aspx` — remove inline `<style>`, use shared `Content/Site.css` classes (3.6).

---

### Task 1: Proyecto temporal de tests (Negocio.Tests)

**Files:**
- Create: `TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj`
- Create: `TPC-Comercio/Negocio.Tests/SmokeTest.cs`

**Interfaces:**
- Consumes: `Negocio.Seguridad.SesionActiva(object)` (already exists) — used only to prove the project references `Negocio` correctly.
- Produces: a working `dotnet test` pipeline that Task 2 will use for real TDD.

- [ ] **Step 1: Create the test project file**

```xml
<!-- TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MSTest.TestFramework" Version="3.6.4" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.6.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Negocio\Negocio.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Write a smoke test to validate the reference to `Negocio` works**

```csharp
// TPC-Comercio/Negocio.Tests/SmokeTest.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Negocio.Tests
{
    [TestClass]
    public class SmokeTest
    {
        [TestMethod]
        public void Seguridad_SesionActiva_NullUsuario_ReturnsFalse()
        {
            Assert.IsFalse(Seguridad.SesionActiva(null));
        }
    }
}
```

- [ ] **Step 3: Run the test and verify it passes**

Run: `dotnet test TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj`
Expected: `Superado: 1` (1 passed), 0 failed.

- [ ] **Step 4: Commit**

```bash
git add TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj TPC-Comercio/Negocio.Tests/SmokeTest.cs
git commit -m "test: scaffold temporary Negocio.Tests project"
```

---

### Task 2: TDD — `PagoNegocio.CalcularPlanDeCuotas`

**Files:**
- Create: `TPC-Comercio/Negocio/PagoNegocio.cs` (only the pure calculation method in this task; the rest is added in Task 5)
- Modify: `TPC-Comercio/Negocio/Negocio.csproj` (add `<Compile Include="PagoNegocio.cs" />`)
- Create: `TPC-Comercio/Negocio.Tests/PagoNegocioTests.cs`

**Interfaces:**
- Produces: `public static List<Cuota> PagoNegocio.CalcularPlanDeCuotas(decimal totalFactura, int cantidadCuotas, decimal tasaMensual, DateTime fechaVenta)` — used by Task 5's `RegistrarPago` and nowhere else yet. Returns one `Dominio.Cuota` per installment with `NroCuota`, `Monto` (capital), `Interes`, `Vencimiento` set; `Id`/`IdPago` left at their default (`0`), filled in by the caller after insert.
- Consumes: `Dominio.Cuota` — **does not exist yet**. Add a minimal version now (full version, unchanged, arrives in Task 4); this task only needs `NroCuota`, `Monto`, `Interes`, `Vencimiento`.

- [ ] **Step 1: Add a minimal `Cuota` domain class (completed fully in Task 4)**

```csharp
// TPC-Comercio/Dominio/Cuota.cs
using System;

namespace Dominio
{
    public class Cuota
    {
        public int Id { get; set; }
        public int IdPago { get; set; }
        public int NroCuota { get; set; }
        public decimal Monto { get; set; }
        public decimal Interes { get; set; }
        public DateTime Vencimiento { get; set; }
    }
}
```

`Dominio.csproj` is a classic (non-SDK) project with an explicit, alphabetically-ordered `<Compile Include>` list — new files are **not** picked up automatically. Add `<Compile Include="Cuota.cs" />` to that `<ItemGroup>`, alphabetically between `<Compile Include="Compra.cs" />` and `<Compile Include="DetalleCompra.cs" />`.

- [ ] **Step 2: Write the failing tests**

```csharp
// TPC-Comercio/Negocio.Tests/PagoNegocioTests.cs
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Negocio;

namespace Negocio.Tests
{
    [TestClass]
    public class PagoNegocioTests
    {
        [TestMethod]
        public void CalcularPlanDeCuotas_3Cuotas_DistribuyeCapitalEInteresConRedondeoEnLaUltima()
        {
            var fechaVenta = new DateTime(2026, 7, 17);

            var plan = PagoNegocio.CalcularPlanDeCuotas(100000m, 3, 5m, fechaVenta);

            Assert.AreEqual(3, plan.Count);

            Assert.AreEqual(1, plan[0].NroCuota);
            Assert.AreEqual(33333.33m, plan[0].Monto);
            Assert.AreEqual(5000.00m, plan[0].Interes);
            Assert.AreEqual(new DateTime(2026, 8, 17), plan[0].Vencimiento);

            Assert.AreEqual(2, plan[1].NroCuota);
            Assert.AreEqual(33333.33m, plan[1].Monto);
            Assert.AreEqual(5000.00m, plan[1].Interes);
            Assert.AreEqual(new DateTime(2026, 9, 17), plan[1].Vencimiento);

            Assert.AreEqual(3, plan[2].NroCuota);
            Assert.AreEqual(33333.34m, plan[2].Monto);
            Assert.AreEqual(5000.00m, plan[2].Interes);
            Assert.AreEqual(new DateTime(2026, 10, 17), plan[2].Vencimiento);
        }

        [TestMethod]
        public void CalcularPlanDeCuotas_12Cuotas_LaSumaDeCapitalEInteresCoincideConElTotal()
        {
            var fechaVenta = new DateTime(2026, 7, 17);

            var plan = PagoNegocio.CalcularPlanDeCuotas(70000m, 12, 5m, fechaVenta);

            Assert.AreEqual(12, plan.Count);
            Assert.AreEqual(70000m, plan.Sum(c => c.Monto));
            Assert.AreEqual(70000m * 0.05m * 12, plan.Sum(c => c.Interes));
            Assert.AreEqual(new DateTime(2027, 7, 17), plan[11].Vencimiento);
        }
    }
}
```

- [ ] **Step 3: Run the tests to verify they fail (compile error expected — `CalcularPlanDeCuotas` doesn't exist yet)**

Run: `dotnet test TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj`
Expected: build FAILS with `CS0117: 'PagoNegocio' does not contain a definition for 'CalcularPlanDeCuotas'` (or similar, since `PagoNegocio` doesn't exist yet either the first time — that's fine, it confirms the test is exercising real, not-yet-written code).

- [ ] **Step 4: Write the minimal implementation**

```csharp
// TPC-Comercio/Negocio/PagoNegocio.cs
using System;
using System.Collections.Generic;
using Dominio;

namespace Negocio
{
    public class PagoNegocio
    {
        public static List<Cuota> CalcularPlanDeCuotas(decimal totalFactura, int cantidadCuotas, decimal tasaMensual, DateTime fechaVenta)
        {
            decimal interesTotal = totalFactura * (tasaMensual / 100m) * cantidadCuotas;
            decimal capitalPorCuota = Math.Round(totalFactura / cantidadCuotas, 2, MidpointRounding.AwayFromZero);
            decimal interesPorCuota = Math.Round(interesTotal / cantidadCuotas, 2, MidpointRounding.AwayFromZero);

            List<Cuota> plan = new List<Cuota>();
            for (int nro = 1; nro <= cantidadCuotas; nro++)
            {
                bool esUltima = nro == cantidadCuotas;

                Cuota cuota = new Cuota();
                cuota.NroCuota = nro;
                cuota.Monto = esUltima ? totalFactura - capitalPorCuota * (cantidadCuotas - 1) : capitalPorCuota;
                cuota.Interes = esUltima ? interesTotal - interesPorCuota * (cantidadCuotas - 1) : interesPorCuota;
                cuota.Vencimiento = fechaVenta.AddMonths(nro);

                plan.Add(cuota);
            }

            return plan;
        }
    }
}
```

Add `<Compile Include="PagoNegocio.cs" />` to `TPC-Comercio/Negocio/Negocio.csproj`'s existing `<ItemGroup>` of `<Compile Include>` entries (alphabetical, next to `MarcaNegocio.cs`).

- [ ] **Step 5: Run the tests to verify they pass**

Run: `dotnet test TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj`
Expected: `Superado: 3` (the smoke test + 2 new tests), 0 failed.

- [ ] **Step 6: Commit**

```bash
git add TPC-Comercio/Dominio/Cuota.cs TPC-Comercio/Dominio/Dominio.csproj TPC-Comercio/Negocio/PagoNegocio.cs TPC-Comercio/Negocio/Negocio.csproj TPC-Comercio/Negocio.Tests/PagoNegocioTests.cs
git commit -m "feat: calculate cuotas plan with simple monthly interest (TDD)"
```

---

### Task 3: Migración SQL — tablas y SPs de Pagos/Cuotas/Caja

**Files:**
- Create: `TPC-Comercio/Database/migrations/02_PagosCuotasCaja.sql`

**Interfaces:**
- Produces: tables `Pagos`, `Cuotas`, `Caja`; stored procedures `SP_Pagos_Crear`, `SP_Pagos_ObtenerPorFactura`, `SP_Pagos_SumarEfectivoDesde`, `SP_Cuotas_Crear`, `SP_Cuotas_ListarPorPago`, `SP_Caja_Abrir`, `SP_Caja_ObtenerAbierta`, `SP_Caja_Cerrar`, `SP_Caja_Listar` — all consumed by Task 5 (`PagoNegocio`) and Task 6 (`CajaNegocio`).

- [ ] **Step 1: Write the migration script**

```sql
-- TPC-Comercio/Database/migrations/02_PagosCuotasCaja.sql
-- Script de migracion: Pagos, Cuotas y Caja
-- Ejecutar contra tpc_P3 en orden, despues de 01_Lotes.sql
USE [tpc_P3]
GO

-- 1. Tabla Pagos
CREATE TABLE [dbo].[Pagos] (
    [Id]             [int]           IDENTITY(1,1) NOT NULL,
    [IdFactura]      [int]           NOT NULL,
    [Tipo]           [varchar](20)   NOT NULL,
    [Monto]          [decimal](12,2) NOT NULL,
    [CantidadCuotas] [int]           NOT NULL DEFAULT (0),
    [Fecha]          [datetime]      NOT NULL DEFAULT (getdate()),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pagos_Factura] FOREIGN KEY([IdFactura]) REFERENCES [dbo].[Facturas] ([Id])
) ON [PRIMARY]
GO

-- 2. Tabla Cuotas
CREATE TABLE [dbo].[Cuotas] (
    [Id]          [int]           IDENTITY(1,1) NOT NULL,
    [IdPago]      [int]           NOT NULL,
    [NroCuota]    [int]           NOT NULL,
    [Monto]       [decimal](10,2) NOT NULL,
    [Interes]     [decimal](10,2) NOT NULL,
    [Vencimiento] [datetime]      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cuotas_Pago] FOREIGN KEY([IdPago]) REFERENCES [dbo].[Pagos] ([Id])
) ON [PRIMARY]
GO

-- 3. Tabla Caja
CREATE TABLE [dbo].[Caja] (
    [Id]                   [int]           IDENTITY(1,1) NOT NULL,
    [FechaApertura]        [datetime]      NOT NULL DEFAULT (getdate()),
    [MontoApertura]        [decimal](12,2) NOT NULL,
    [IdUsuarioApertura]    [int]           NOT NULL,
    [FechaCierre]          [datetime]      NULL,
    [MontoCierreDeclarado] [decimal](12,2) NULL,
    [MontoCierreCalculado] [decimal](12,2) NULL,
    [IdUsuarioCierre]      [int]           NULL,
    [Estado]               [bit]           NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Caja_UsuarioApertura] FOREIGN KEY([IdUsuarioApertura]) REFERENCES [dbo].[Usuarios] ([Id]),
    CONSTRAINT [FK_Caja_UsuarioCierre]   FOREIGN KEY([IdUsuarioCierre])   REFERENCES [dbo].[Usuarios] ([Id])
) ON [PRIMARY]
GO

-- 4. SPs de Pagos
CREATE OR ALTER PROCEDURE [dbo].[SP_Pagos_Crear]
    @IdFactura      int,
    @Tipo           varchar(20),
    @Monto          decimal(12,2),
    @CantidadCuotas int,
    @NewId          int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[Pagos] (IdFactura, Tipo, Monto, CantidadCuotas)
    VALUES (@IdFactura, @Tipo, @Monto, @CantidadCuotas)
    SET @NewId = SCOPE_IDENTITY()
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Pagos_ObtenerPorFactura]
    @IdFactura int
AS
BEGIN
    SELECT Id, IdFactura, Tipo, Monto, CantidadCuotas, Fecha
    FROM [dbo].[Pagos]
    WHERE IdFactura = @IdFactura
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Pagos_SumarEfectivoDesde]
    @Desde datetime
AS
BEGIN
    SELECT ISNULL(SUM(Monto), 0) AS Total
    FROM [dbo].[Pagos]
    WHERE Tipo = 'Efectivo' AND Fecha >= @Desde
END
GO

-- 5. SPs de Cuotas
CREATE OR ALTER PROCEDURE [dbo].[SP_Cuotas_Crear]
    @IdPago      int,
    @NroCuota    int,
    @Monto       decimal(10,2),
    @Interes     decimal(10,2),
    @Vencimiento datetime
AS
BEGIN
    INSERT INTO [dbo].[Cuotas] (IdPago, NroCuota, Monto, Interes, Vencimiento)
    VALUES (@IdPago, @NroCuota, @Monto, @Interes, @Vencimiento)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Cuotas_ListarPorPago]
    @IdPago int
AS
BEGIN
    SELECT Id, IdPago, NroCuota, Monto, Interes, Vencimiento
    FROM [dbo].[Cuotas]
    WHERE IdPago = @IdPago
    ORDER BY NroCuota ASC
END
GO

-- 6. SPs de Caja
CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_Abrir]
    @MontoApertura decimal(12,2),
    @IdUsuario     int,
    @NewId         int OUTPUT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM [dbo].[Caja] WHERE Estado = 1)
    BEGIN
        RAISERROR('Ya hay una caja abierta.', 16, 1)
        RETURN
    END

    INSERT INTO [dbo].[Caja] (MontoApertura, IdUsuarioApertura)
    VALUES (@MontoApertura, @IdUsuario)
    SET @NewId = SCOPE_IDENTITY()
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_ObtenerAbierta]
AS
BEGIN
    SELECT
        c.Id, c.FechaApertura, c.MontoApertura, c.IdUsuarioApertura, ua.[User] AS UsuarioApertura,
        c.FechaCierre, c.MontoCierreDeclarado, c.MontoCierreCalculado, c.IdUsuarioCierre, uc.[User] AS UsuarioCierre,
        c.Estado
    FROM [dbo].[Caja] c
    INNER JOIN [dbo].[Usuarios] ua ON c.IdUsuarioApertura = ua.Id
    LEFT JOIN  [dbo].[Usuarios] uc ON c.IdUsuarioCierre   = uc.Id
    WHERE c.Estado = 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_Cerrar]
    @Id                   int,
    @MontoCierreDeclarado decimal(12,2),
    @IdUsuario            int
AS
BEGIN
    DECLARE @FechaApertura datetime
    DECLARE @MontoCalculado decimal(12,2)

    SELECT @FechaApertura = FechaApertura FROM [dbo].[Caja] WHERE Id = @Id AND Estado = 1

    IF @FechaApertura IS NULL
    BEGIN
        RAISERROR('No se encontro una caja abierta con el Id indicado.', 16, 1)
        RETURN
    END

    SELECT @MontoCalculado = ISNULL(SUM(Monto), 0)
    FROM [dbo].[Pagos]
    WHERE Tipo = 'Efectivo' AND Fecha BETWEEN @FechaApertura AND getdate()

    UPDATE [dbo].[Caja]
    SET FechaCierre = getdate(),
        MontoCierreDeclarado = @MontoCierreDeclarado,
        MontoCierreCalculado = @MontoCalculado,
        IdUsuarioCierre = @IdUsuario,
        Estado = 0
    WHERE Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_Listar]
AS
BEGIN
    SELECT
        c.Id, c.FechaApertura, c.MontoApertura, c.IdUsuarioApertura, ua.[User] AS UsuarioApertura,
        c.FechaCierre, c.MontoCierreDeclarado, c.MontoCierreCalculado, c.IdUsuarioCierre, uc.[User] AS UsuarioCierre,
        c.Estado
    FROM [dbo].[Caja] c
    INNER JOIN [dbo].[Usuarios] ua ON c.IdUsuarioApertura = ua.Id
    LEFT JOIN  [dbo].[Usuarios] uc ON c.IdUsuarioCierre   = uc.Id
    ORDER BY c.FechaApertura DESC
END
GO
```

- [ ] **Step 2: Run the migration against the local dev database**

Run (adjust `-S` to your instance if different from the one hardcoded in `AccesoDatos.cs`):
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -i "C:\UTN\TPC-Comercio\TPC-Comercio\Database\migrations\02_PagosCuotasCaja.sql"
```
Expected: no errors printed; each `GO` batch completes silently.

- [ ] **Step 3: Verify the tables and procedures exist**

Run:
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "SELECT name FROM sys.tables WHERE name IN ('Pagos','Cuotas','Caja'); SELECT name FROM sys.procedures WHERE name LIKE 'SP_Pagos_%' OR name LIKE 'SP_Cuotas_%' OR name LIKE 'SP_Caja_%';"
```
Expected: 3 table names and 9 procedure names listed.

- [ ] **Step 4: Verify the "only one open Caja" rule manually**

Run:
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DECLARE @Id int; EXEC SP_Caja_Abrir @MontoApertura = 1000, @IdUsuario = 1, @NewId = @Id OUTPUT; SELECT @Id AS PrimeraCaja; EXEC SP_Caja_Abrir @MontoApertura = 500, @IdUsuario = 1, @NewId = @Id OUTPUT;"
```
Expected: the first `EXEC` succeeds and prints a new `Id`; the second `EXEC` fails with "Ya hay una caja abierta." Then clean up the test row so Task 9's manual testing starts from a clean state:
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DELETE FROM Caja WHERE MontoApertura = 1000 AND MontoCierreDeclarado IS NULL;"
```

- [ ] **Step 5: Commit**

```bash
git add TPC-Comercio/Database/migrations/02_PagosCuotasCaja.sql
git commit -m "feat: add Pagos/Cuotas/Caja tables and stored procedures"
```

---

### Task 4: Dominio — clases Pago y Caja (Cuota ya existe desde Task 2)

**Files:**
- Create: `TPC-Comercio/Dominio/Pago.cs`
- Create: `TPC-Comercio/Dominio/Caja.cs`
- Modify: `TPC-Comercio/Dominio/Dominio.csproj`

**Interfaces:**
- Produces: `Dominio.Pago` (Id, IdFactura, Tipo, Monto, CantidadCuotas, Fecha) — consumed by Task 5's `PagoNegocio.ObtenerPorFactura` and Task 8's `ConfirmacionVenta.aspx.cs`.
- Produces: `Dominio.Caja` (Id, FechaApertura, MontoApertura, IdUsuarioApertura, UsuarioApertura, FechaCierre, MontoCierreDeclarado, MontoCierreCalculado, IdUsuarioCierre, UsuarioCierre, Estado, computed `Diferencia`) — consumed by Task 6's `CajaNegocio` and Task 9's `Caja.aspx.cs`. `UsuarioApertura`/`UsuarioCierre` are display-only fields populated by a SQL join, same pattern as `Lote.Proveedor` in the existing `LoteNegocio.ListarPorProducto`.

- [ ] **Step 1: Add `Pago.cs`**

```csharp
// TPC-Comercio/Dominio/Pago.cs
using System;

namespace Dominio
{
    public class Pago
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public int CantidadCuotas { get; set; }
        public DateTime Fecha { get; set; }
    }
}
```

- [ ] **Step 2: Add `Caja.cs`**

```csharp
// TPC-Comercio/Dominio/Caja.cs
using System;

namespace Dominio
{
    public class Caja
    {
        public int Id { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal MontoApertura { get; set; }
        public int IdUsuarioApertura { get; set; }
        public string UsuarioApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal? MontoCierreDeclarado { get; set; }
        public decimal? MontoCierreCalculado { get; set; }
        public int? IdUsuarioCierre { get; set; }
        public string UsuarioCierre { get; set; }
        public bool Estado { get; set; }

        public decimal? Diferencia
        {
            get
            {
                if (MontoCierreDeclarado.HasValue && MontoCierreCalculado.HasValue)
                {
                    return MontoCierreDeclarado.Value - MontoCierreCalculado.Value;
                }
                return null;
            }
        }
    }
}
```

- [ ] **Step 3: Register both files in `Dominio.csproj`**

Add `<Compile Include="Caja.cs" />` alphabetically between `<Compile Include="Categoria.cs" />` and `<Compile Include="Cliente.cs" />`, and `<Compile Include="Pago.cs" />` alphabetically between `<Compile Include="Marca.cs" />` and `<Compile Include="Perfil.cs" />`.

- [ ] **Step 4: Build to verify it compiles**

Run: `dotnet build TPC-Comercio/Dominio/Dominio.csproj`
Expected: `Compilación correcta. 0 Errores`.

- [ ] **Step 5: Commit**

```bash
git add TPC-Comercio/Dominio/Pago.cs TPC-Comercio/Dominio/Caja.cs TPC-Comercio/Dominio/Dominio.csproj
git commit -m "feat: add Pago and Caja domain classes"
```

---

### Task 5: `PagoNegocio` — acceso a datos (RegistrarPago, ObtenerPorFactura, ListarCuotasPorPago, SumarEfectivoDesde)

**Files:**
- Modify: `TPC-Comercio/Negocio/PagoNegocio.cs` (extends the class created in Task 2)
- Modify: `TPC-Comercio/AplicacionWebComercio/Web.config`

**Interfaces:**
- Consumes: `SP_Pagos_Crear`, `SP_Pagos_ObtenerPorFactura`, `SP_Pagos_SumarEfectivoDesde`, `SP_Cuotas_Crear`, `SP_Cuotas_ListarPorPago` (Task 3); `Dominio.Pago`, `Dominio.Cuota` (Task 2/4); `PagoNegocio.CalcularPlanDeCuotas` (Task 2).
- Produces: `int RegistrarPago(int idFactura, decimal totalFactura, string tipoPago, int cantidadCuotas)`, `Pago ObtenerPorFactura(int idFactura)`, `List<Cuota> ListarCuotasPorPago(int idPago)`, `decimal SumarEfectivoDesde(DateTime desde)` — consumed by Task 7 (`VentasFormulario`), Task 8 (`ConfirmacionVenta`), Task 9 (`Caja.aspx`).

- [ ] **Step 1: Add the `TasaInteresMensual` app setting**

`TPC-Comercio/AplicacionWebComercio/Web.config` has no `<appSettings>` section today. Add one right after `<globalization ... />` closes, inside `<system.web>`'s parent `<configuration>` (as a sibling of `<system.web>`):

```xml
  <appSettings>
    <add key="TasaInteresMensual" value="5" />
  </appSettings>
  <system.web>
```

- [ ] **Step 2: Extend `PagoNegocio.cs` with the data-access methods**

```csharp
// TPC-Comercio/Negocio/PagoNegocio.cs (full file after this task)
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class PagoNegocio
    {
        public static List<Cuota> CalcularPlanDeCuotas(decimal totalFactura, int cantidadCuotas, decimal tasaMensual, DateTime fechaVenta)
        {
            decimal interesTotal = totalFactura * (tasaMensual / 100m) * cantidadCuotas;
            decimal capitalPorCuota = Math.Round(totalFactura / cantidadCuotas, 2, MidpointRounding.AwayFromZero);
            decimal interesPorCuota = Math.Round(interesTotal / cantidadCuotas, 2, MidpointRounding.AwayFromZero);

            List<Cuota> plan = new List<Cuota>();
            for (int nro = 1; nro <= cantidadCuotas; nro++)
            {
                bool esUltima = nro == cantidadCuotas;

                Cuota cuota = new Cuota();
                cuota.NroCuota = nro;
                cuota.Monto = esUltima ? totalFactura - capitalPorCuota * (cantidadCuotas - 1) : capitalPorCuota;
                cuota.Interes = esUltima ? interesTotal - interesPorCuota * (cantidadCuotas - 1) : interesPorCuota;
                cuota.Vencimiento = fechaVenta.AddMonths(nro);

                plan.Add(cuota);
            }

            return plan;
        }

        public int RegistrarPago(int idFactura, decimal totalFactura, string tipoPago, int cantidadCuotas)
        {
            List<Cuota> plan = new List<Cuota>();
            decimal montoPago = totalFactura;

            if (tipoPago == "Credito")
            {
                decimal tasaMensual = decimal.Parse(ConfigurationManager.AppSettings["TasaInteresMensual"], CultureInfo.InvariantCulture);
                plan = CalcularPlanDeCuotas(totalFactura, cantidadCuotas, tasaMensual, DateTime.Now);
                montoPago = totalFactura + plan.Sum(c => c.Interes);
            }

            int idPago = Crear(idFactura, tipoPago, montoPago, tipoPago == "Credito" ? cantidadCuotas : 0);

            foreach (Cuota cuota in plan)
            {
                InsertarCuota(idPago, cuota);
            }

            return idPago;
        }

        private int Crear(int idFactura, string tipo, decimal monto, int cantidadCuotas)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Pagos_Crear");
                datos.setearParametro("@IdFactura", idFactura);
                datos.setearParametro("@Tipo", tipo);
                datos.setearParametro("@Monto", monto);
                datos.setearParametro("@CantidadCuotas", cantidadCuotas);
                datos.setearParametroSalida("@NewId", System.Data.SqlDbType.Int);
                datos.ejecutarAccion();
                return Convert.ToInt32(datos.obtenerParametro("@NewId"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private void InsertarCuota(int idPago, Cuota cuota)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Cuotas_Crear");
                datos.setearParametro("@IdPago", idPago);
                datos.setearParametro("@NroCuota", cuota.NroCuota);
                datos.setearParametro("@Monto", cuota.Monto);
                datos.setearParametro("@Interes", cuota.Interes);
                datos.setearParametro("@Vencimiento", cuota.Vencimiento);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Pago ObtenerPorFactura(int idFactura)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Pagos_ObtenerPorFactura");
                datos.setearParametro("@IdFactura", idFactura);
                datos.ejecutarLectura();

                Pago pago = null;
                if (datos.Lector.Read())
                {
                    pago = new Pago();
                    pago.Id = (int)datos.Lector["Id"];
                    pago.IdFactura = (int)datos.Lector["IdFactura"];
                    pago.Tipo = (string)datos.Lector["Tipo"];
                    pago.Monto = (decimal)datos.Lector["Monto"];
                    pago.CantidadCuotas = (int)datos.Lector["CantidadCuotas"];
                    pago.Fecha = (DateTime)datos.Lector["Fecha"];
                }
                return pago;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Cuota> ListarCuotasPorPago(int idPago)
        {
            List<Cuota> lista = new List<Cuota>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Cuotas_ListarPorPago");
                datos.setearParametro("@IdPago", idPago);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Cuota aux = new Cuota();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.IdPago = idPago;
                    aux.NroCuota = (int)datos.Lector["NroCuota"];
                    aux.Monto = (decimal)datos.Lector["Monto"];
                    aux.Interes = (decimal)datos.Lector["Interes"];
                    aux.Vencimiento = (DateTime)datos.Lector["Vencimiento"];
                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public decimal SumarEfectivoDesde(DateTime desde)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Pagos_SumarEfectivoDesde");
                datos.setearParametro("@Desde", desde);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return (decimal)datos.Lector["Total"];
                }
                return 0m;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
```

- [ ] **Step 3: Build to verify it compiles**

Run: `dotnet build TPC-Comercio/Negocio/Negocio.csproj`
Expected: `Compilación correcta. 0 Errores`.

- [ ] **Step 4: Re-run the Task 2 unit tests to confirm nothing broke**

Run: `dotnet test TPC-Comercio/Negocio.Tests/Negocio.Tests.csproj`
Expected: `Superado: 3`, 0 failed.

- [ ] **Step 5: Manual verification against the dev database**

There's no existing test Cliente/Factura fixture helper in this codebase, so verify by hand using the seed data already in `Facturas` (Id 1, Total 270000.00, per `TPC_Comercio.sql`):

```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "EXEC SP_Pagos_Crear @IdFactura=1, @Tipo='Credito', @Monto=310500.00, @CantidadCuotas=3, @NewId=NULL; SELECT * FROM Pagos WHERE IdFactura = 1;"
```
Expected: a `Pagos` row with `Tipo='Credito'`, `Monto=310500.00`, `CantidadCuotas=3`. Clean up afterward:
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DELETE FROM Pagos WHERE IdFactura = 1;"
```

- [ ] **Step 6: Commit**

```bash
git add TPC-Comercio/Negocio/PagoNegocio.cs TPC-Comercio/AplicacionWebComercio/Web.config
git commit -m "feat: PagoNegocio data access (RegistrarPago, ObtenerPorFactura, cuotas)"
```

---

### Task 6: `CajaNegocio`

**Files:**
- Create: `TPC-Comercio/Negocio/CajaNegocio.cs`
- Modify: `TPC-Comercio/Negocio/Negocio.csproj`

**Interfaces:**
- Consumes: `SP_Caja_Abrir`, `SP_Caja_ObtenerAbierta`, `SP_Caja_Cerrar`, `SP_Caja_Listar` (Task 3); `Dominio.Caja` (Task 4).
- Produces: `int AbrirCaja(decimal montoApertura, int idUsuario)`, `Caja ObtenerCajaAbierta()`, `void CerrarCaja(int idCaja, decimal montoCierreDeclarado, int idUsuario)`, `List<Caja> Listar()` — consumed by Task 9 (`Caja.aspx.cs`).

- [ ] **Step 1: Write `CajaNegocio.cs`**

```csharp
// TPC-Comercio/Negocio/CajaNegocio.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dominio;
using AccesoDatos;

namespace Negocio
{
    public class CajaNegocio
    {
        public int AbrirCaja(decimal montoApertura, int idUsuario)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_Abrir");
                datos.setearParametro("@MontoApertura", montoApertura);
                datos.setearParametro("@IdUsuario", idUsuario);
                datos.setearParametroSalida("@NewId", System.Data.SqlDbType.Int);
                datos.ejecutarAccion();
                return Convert.ToInt32(datos.obtenerParametro("@NewId"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Caja ObtenerCajaAbierta()
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_ObtenerAbierta");
                datos.ejecutarLectura();
                Caja caja = null;
                if (datos.Lector.Read())
                {
                    caja = MapearCaja(datos.Lector);
                }
                return caja;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void CerrarCaja(int idCaja, decimal montoCierreDeclarado, int idUsuario)
        {
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_Cerrar");
                datos.setearParametro("@Id", idCaja);
                datos.setearParametro("@MontoCierreDeclarado", montoCierreDeclarado);
                datos.setearParametro("@IdUsuario", idUsuario);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Caja> Listar()
        {
            List<Caja> lista = new List<Caja>();
            AccesoDatos.AccesoDatos datos = new AccesoDatos.AccesoDatos();
            try
            {
                datos.setearProcedimiento("SP_Caja_Listar");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(MapearCaja(datos.Lector));
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private Caja MapearCaja(SqlDataReader lector)
        {
            Caja caja = new Caja();
            caja.Id = (int)lector["Id"];
            caja.FechaApertura = (DateTime)lector["FechaApertura"];
            caja.MontoApertura = (decimal)lector["MontoApertura"];
            caja.IdUsuarioApertura = (int)lector["IdUsuarioApertura"];
            caja.UsuarioApertura = (string)lector["UsuarioApertura"];
            caja.FechaCierre = lector["FechaCierre"] != DBNull.Value ? (DateTime?)lector["FechaCierre"] : null;
            caja.MontoCierreDeclarado = lector["MontoCierreDeclarado"] != DBNull.Value ? (decimal?)lector["MontoCierreDeclarado"] : null;
            caja.MontoCierreCalculado = lector["MontoCierreCalculado"] != DBNull.Value ? (decimal?)lector["MontoCierreCalculado"] : null;
            caja.IdUsuarioCierre = lector["IdUsuarioCierre"] != DBNull.Value ? (int?)lector["IdUsuarioCierre"] : null;
            caja.UsuarioCierre = lector["UsuarioCierre"] != DBNull.Value ? (string)lector["UsuarioCierre"] : null;
            caja.Estado = (bool)lector["Estado"];
            return caja;
        }
    }
}
```

Add `<Compile Include="CajaNegocio.cs" />` to `Negocio.csproj`.

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build TPC-Comercio/Negocio/Negocio.csproj`
Expected: `Compilación correcta. 0 Errores`.

- [ ] **Step 3: Manual verification — apertura, doble apertura bloqueada, cierre con arqueo**

```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DECLARE @Id int; EXEC SP_Caja_Abrir @MontoApertura=1000, @IdUsuario=1, @NewId=@Id OUTPUT; SELECT * FROM Caja WHERE Estado = 1;"
```
Expected: one row, `MontoApertura=1000`, `Estado=1`.
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "EXEC SP_Caja_Cerrar @Id=(SELECT TOP 1 Id FROM Caja WHERE Estado=1), @MontoCierreDeclarado=1000, @IdUsuario=1; SELECT * FROM Caja ORDER BY Id DESC;"
```
Expected: the row now has `Estado=0`, `MontoCierreDeclarado=1000`, `MontoCierreCalculado=0` (no `Pagos` rows in the window). Clean up:
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DELETE FROM Caja WHERE MontoApertura = 1000;"
```

- [ ] **Step 4: Commit**

```bash
git add TPC-Comercio/Negocio/CajaNegocio.cs TPC-Comercio/Negocio/Negocio.csproj
git commit -m "feat: CajaNegocio (abrir, cerrar, listar historico)"
```

---

### Task 7: `VentasFormulario` — forma de pago + wiring a PagoNegocio

**Files:**
- Modify: `TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx`
- Modify: `TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx.cs`
- Modify: `TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx.designer.cs`

**Interfaces:**
- Consumes: `PagoNegocio.RegistrarPago` (Task 5). `ConfirmacionVenta.aspx` (Task 8) becomes the new redirect target — **Task 8 must exist (or at least the page must be present) before this task's manual verification step can succeed**; if doing tasks in order, do Task 8 first or accept a 404 on this step until Task 8 lands. Given the dependency, **do Task 8 before running this task's Step 5.**
- Produces: nothing new consumed by later tasks besides the fact that every sale now has a `Pagos` row.

- [ ] **Step 1: Add the forma de pago UI to `VentasFormulario.aspx`**

Insert this block right after the "Cliente" `div` and before the `<hr />` that precedes "Agregar productos":

```aspx
            <%-- Forma de pago --%>
            <div class="mb-3 row align-items-center">
                <label class="col-sm-2 col-form-label fw-semibold">Forma de pago</label>
                <div class="col-sm-3">
                    <asp:DropDownList ID="ddlFormaPago" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Efectivo" Value="Efectivo" Selected="True" />
                        <asp:ListItem Text="Débito" Value="Debito" />
                        <asp:ListItem Text="Crédito" Value="Credito" />
                    </asp:DropDownList>
                </div>
                <div class="col-sm-3" id="divCantidadCuotas" style="display:none;">
                    <asp:DropDownList ID="ddlCantidadCuotas" runat="server" CssClass="form-select">
                        <asp:ListItem Text="3 cuotas" Value="3" />
                        <asp:ListItem Text="6 cuotas" Value="6" />
                        <asp:ListItem Text="12 cuotas" Value="12" />
                    </asp:DropDownList>
                </div>
            </div>

            <script type="text/javascript">
                document.addEventListener("DOMContentLoaded", function () {
                    var ddlFormaPago = document.getElementById("<%= ddlFormaPago.ClientID %>");
                    var divCuotas = document.getElementById("divCantidadCuotas");

                    function actualizarVisibilidadCuotas() {
                        divCuotas.style.display = (ddlFormaPago.value === "Credito") ? "" : "none";
                    }

                    ddlFormaPago.addEventListener("change", actualizarVisibilidadCuotas);
                    actualizarVisibilidadCuotas();
                });
            </script>
```

- [ ] **Step 2: Add the designer fields**

In `VentasFormulario.aspx.designer.cs`, add (matching the existing style of that file):

```csharp
        protected global::System.Web.UI.WebControls.DropDownList ddlFormaPago;

        protected global::System.Web.UI.WebControls.DropDownList ddlCantidadCuotas;
```

- [ ] **Step 3: Wire `btnGuardar_Click` to `PagoNegocio` and redirect to `ConfirmacionVenta.aspx`**

In `VentasFormulario.aspx.cs`, add `using System.Linq;` to the usings, then replace `btnGuardar_Click`:

```csharp
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            List<DetalleFactura> items = (List<DetalleFactura>)Session["itemsVenta"];

            if (items.Count == 0)
            {
                MostrarMensaje("Debe agregar al menos un producto al carrito.", false);
                return;
            }

            Factura factura = new Factura();
            factura.Cliente = new Cliente { ID = int.Parse(ddlCliente.SelectedValue) };
            factura.Usuario = new Usuario { Id = ((Usuario)Session["Usuario"]).Id };
            try
            {
                int idGenerado = new FacturaNegocio().RegistrarVenta(factura, items);

                Factura facturaCreada = new FacturaNegocio().Listar().FirstOrDefault(f => f.Id == idGenerado);
                string formaPago = ddlFormaPago.SelectedValue;
                int cantidadCuotas = (formaPago == "Credito") ? int.Parse(ddlCantidadCuotas.SelectedValue) : 0;

                new PagoNegocio().RegistrarPago(idGenerado, facturaCreada.Total, formaPago, cantidadCuotas);

                Session.Remove("itemsVenta");
                Response.Redirect("ConfirmacionVenta.aspx?id=" + idGenerado, false);
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, false);
            }
        }
```

- [ ] **Step 4: Build to verify it compiles**

Run (see Global Constraints — `dotnet build` cannot build this project):
```bash
MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
```
Expected: no `error` lines; last output lines show each project's `.dll` output path (e.g. `AplicacionWebComercio -> ...\bin\AplicacionWebComercio.dll`).

- [ ] **Step 5: Verify the page loads (do this after Task 8 is done)**

Start IIS Express per Global Constraints, then `WebFetch` on `http://localhost:59928/VentasFormulario.aspx` and confirm the response contains "Forma de pago", "Efectivo", "Débito", "Crédito" and no server error text (e.g. no "Server Error in" / stack trace). This confirms the new controls render; it does **not** exercise `btnGuardar_Click` (that needs a real form POST — leave it in the report as: "full sale flow (choose Crédito, submit, land on ConfirmacionVenta) needs manual QA by the user").

- [ ] **Step 6: Commit**

```bash
git add TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx.cs TPC-Comercio/AplicacionWebComercio/VentasFormulario.aspx.designer.cs
git commit -m "feat: forma de pago en VentasFormulario, registra Pago tras la venta"
```

---

### Task 8: Nueva página `ConfirmacionVenta.aspx`

**Files:**
- Create: `TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx`
- Create: `TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx.cs`
- Create: `TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx.designer.cs`
- Modify: `TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj`

**Interfaces:**
- Consumes: `FacturaNegocio.Listar()`, `DetalleFacturaNegocio.ListarPorFactura(int)` (existing), `PagoNegocio.ObtenerPorFactura(int)`, `PagoNegocio.ListarCuotasPorPago(int)` (Task 5), `Negocio.Seguridad.EsCliente` (existing).
- Produces: the page `ConfirmacionVenta.aspx?id=<idFactura>` that Task 7 redirects to.

- [ ] **Step 1: Write `ConfirmacionVenta.aspx`**

```aspx
<%@ Page Title="Confirmación de venta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConfirmacionVenta.aspx.cs" Inherits="AplicacionWebComercio.ConfirmacionVenta" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">Venta confirmada</h2>
    </div>

    <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-none w-100 mb-3" />

    <asp:Panel ID="pnlResumen" runat="server">
        <div class="card mb-3">
            <div class="card-header">
                <h5 class="mb-0">Datos de la venta</h5>
            </div>
            <div class="card-body">
                <p class="mb-1"><strong>Factura N°:</strong> <asp:Literal ID="litNumeroFactura" runat="server" /></p>
                <p class="mb-1"><strong>Fecha:</strong> <asp:Literal ID="litFecha" runat="server" /></p>
                <p class="mb-1"><strong>Cliente:</strong> <asp:Literal ID="litCliente" runat="server" /></p>
                <p class="mb-0"><strong>Forma de pago:</strong> <asp:Literal ID="litFormaPago" runat="server" /></p>
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <h5 class="mb-0">Productos</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="dgvItems" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm">
                    <Columns>
                        <asp:BoundField DataField="Producto.NombreProducto" HeaderText="Descripción" />
                        <asp:BoundField DataField="Cantidad"    HeaderText="Cant."       ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="PrecioVenta" HeaderText="P. Unitario" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="Subtotal"    HeaderText="Subtotal"    DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    </Columns>
                </asp:GridView>
                <div class="text-end fs-5">
                    Total: <strong><asp:Literal ID="litTotal" runat="server" /></strong>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlCuotas" runat="server" Visible="false" CssClass="card mb-3">
            <div class="card-header">
                <h5 class="mb-0">Plan de cuotas</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="dgvCuotas" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm">
                    <Columns>
                        <asp:BoundField DataField="NroCuota"    HeaderText="Cuota N°" />
                        <asp:BoundField DataField="Monto"       HeaderText="Monto"       DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="Interes"     HeaderText="Interés"     DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="Vencimiento" HeaderText="Vencimiento" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <div class="d-flex justify-content-end gap-2">
            <a runat="server" id="lnkVerFactura" class="btn btn-outline-secondary">Ver / Imprimir factura</a>
            <a href="VentasFormulario.aspx" class="btn btn-outline-secondary">Nueva venta</a>
            <a href="Ventas.aspx" class="btn btn-primary">Volver al listado</a>
        </div>
    </asp:Panel>

</asp:Content>
```

- [ ] **Step 2: Write `ConfirmacionVenta.aspx.cs`**

```csharp
using System;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class ConfirmacionVenta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int idFactura;
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out idFactura))
                {
                    CargarConfirmacion(idFactura);
                }
                else
                {
                    MostrarError("ID de factura inválido.");
                }
            }
        }

        private void CargarConfirmacion(int idFactura)
        {
            try
            {
                FacturaNegocio facturaNegocio = new FacturaNegocio();
                Factura factura = facturaNegocio.Listar().FirstOrDefault(f => f.Id == idFactura);

                if (factura == null)
                {
                    MostrarError("No se encontró la factura solicitada.");
                    return;
                }

                Usuario u = Session["Usuario"] as Usuario;
                if (Seguridad.EsCliente(u) && factura.Cliente.ID != u.IdEntidad)
                {
                    Response.Redirect("Default.aspx", false);
                    return;
                }

                litNumeroFactura.Text = factura.NumeroFactura;
                litFecha.Text = factura.Fecha.ToString("dd/MM/yyyy HH:mm");
                litCliente.Text = factura.Cliente.Nombre + " " + factura.Cliente.Apellido;
                litTotal.Text = factura.Total.ToString("C");

                dgvItems.DataSource = new DetalleFacturaNegocio().ListarPorFactura(idFactura);
                dgvItems.DataBind();

                lnkVerFactura.HRef = "FacturaReporte.aspx?id=" + idFactura;

                Pago pago = new PagoNegocio().ObtenerPorFactura(idFactura);
                if (pago != null)
                {
                    litFormaPago.Text = pago.Tipo;

                    if (pago.Tipo == "Credito")
                    {
                        pnlCuotas.Visible = true;
                        dgvCuotas.DataSource = new PagoNegocio().ListarCuotasPorPago(pago.Id);
                        dgvCuotas.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("Ocurrió un error al cargar la confirmación: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = "alert alert-danger w-100 mb-3";
            pnlResumen.Visible = false;
        }
    }
}
```

- [ ] **Step 3: Write `ConfirmacionVenta.aspx.designer.cs`**

```csharp
namespace AplicacionWebComercio
{
    public partial class ConfirmacionVenta
    {
        protected global::System.Web.UI.WebControls.Label lblMensaje;
        protected global::System.Web.UI.WebControls.Panel pnlResumen;
        protected global::System.Web.UI.Literal litNumeroFactura;
        protected global::System.Web.UI.Literal litFecha;
        protected global::System.Web.UI.Literal litCliente;
        protected global::System.Web.UI.Literal litFormaPago;
        protected global::System.Web.UI.WebControls.GridView dgvItems;
        protected global::System.Web.UI.Literal litTotal;
        protected global::System.Web.UI.WebControls.Panel pnlCuotas;
        protected global::System.Web.UI.WebControls.GridView dgvCuotas;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor lnkVerFactura;
    }
}
```

- [ ] **Step 4: Register the new files in `AplicacionWebComercio.csproj`**

Add to the `<Content>` `<ItemGroup>`: `<Content Include="ConfirmacionVenta.aspx" />`.
Add to the `<Compile>` `<ItemGroup>` (next to `CompraReporte.aspx.cs`'s entries):

```xml
    <Compile Include="ConfirmacionVenta.aspx.cs">
      <DependentUpon>ConfirmacionVenta.aspx</DependentUpon>
      <SubType>ASPXCodeBehind</SubType>
    </Compile>
    <Compile Include="ConfirmacionVenta.aspx.designer.cs">
      <DependentUpon>ConfirmacionVenta.aspx</DependentUpon>
    </Compile>
```

- [ ] **Step 5: Build to verify it compiles**

Run (see Global Constraints — `dotnet build` cannot build this project):
```bash
MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
```
Expected: no `error` lines; last output lines show each project's `.dll` output path (e.g. `AplicacionWebComercio -> ...\bin\AplicacionWebComercio.dll`).

- [ ] **Step 6: Verify the page renders, using seed data directly (no form POST needed)**

This page can be checked with GET requests alone, without going through `VentasFormulario`. Seed a `Pago`+`Cuotas` for the existing `Facturas` row with `Id = 1` (Total 270000.00, from `TPC_Comercio.sql`):
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DECLARE @IdPago int; EXEC SP_Pagos_Crear @IdFactura=1, @Tipo='Credito', @Monto=310500.00, @CantidadCuotas=3, @NewId=@IdPago OUTPUT; SELECT @IdPago AS IdPago;"
```
Note the printed `IdPago`, then insert its 3 cuotas (adjust `@IdPago` to the value just printed):
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "EXEC SP_Cuotas_Crear @IdPago=<IdPago>, @NroCuota=1, @Monto=90000.00, @Interes=13500.00, @Vencimiento='2026-08-17'; EXEC SP_Cuotas_Crear @IdPago=<IdPago>, @NroCuota=2, @Monto=90000.00, @Interes=13500.00, @Vencimiento='2026-09-17'; EXEC SP_Cuotas_Crear @IdPago=<IdPago>, @NroCuota=3, @Monto=90000.00, @Interes=13500.00, @Vencimiento='2026-10-17';"
```
Start IIS Express per Global Constraints, then `WebFetch` on `http://localhost:59928/ConfirmacionVenta.aspx?id=1` and confirm the response shows the factura number, "Credito", a 3-row cuotas table with the values just inserted, a working link to `FacturaReporte.aspx?id=1`, and no server error text. Then clean up:
```bash
sqlcmd -S ".\MSSQLSERVER01" -d tpc_P3 -E -C -Q "DELETE FROM Cuotas WHERE IdPago IN (SELECT Id FROM Pagos WHERE IdFactura = 1); DELETE FROM Pagos WHERE IdFactura = 1;"
```
Leave in the report: "full sale-to-confirmation flow through the UI (VentasFormulario submit → redirect) needs manual QA by the user."

- [ ] **Step 7: Commit**

```bash
git add TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx.cs TPC-Comercio/AplicacionWebComercio/ConfirmacionVenta.aspx.designer.cs TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj
git commit -m "feat: pantalla de confirmacion post-venta"
```

---

### Task 9: Nueva página `Caja.aspx` + nav en `Site.Master`

**Files:**
- Create: `TPC-Comercio/AplicacionWebComercio/Caja.aspx`
- Create: `TPC-Comercio/AplicacionWebComercio/Caja.aspx.cs`
- Create: `TPC-Comercio/AplicacionWebComercio/Caja.aspx.designer.cs`
- Modify: `TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj`
- Modify: `TPC-Comercio/AplicacionWebComercio/Site.Master`
- Modify: `TPC-Comercio/AplicacionWebComercio/Site.Master.cs`
- Modify: `TPC-Comercio/AplicacionWebComercio/Site.Master.designer.cs`

**Interfaces:**
- Consumes: `CajaNegocio` (Task 6), `PagoNegocio.SumarEfectivoDesde` (Task 5), `Negocio.Seguridad.SesionActiva/EsAdmin/EsVendedor` (existing).

- [ ] **Step 1: Write `Caja.aspx`**

```aspx
<%@ Page Title="Caja" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Caja.aspx.cs" Inherits="AplicacionWebComercio.Caja" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">Caja</h2>
    </div>

    <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-none w-100 mb-3" />

    <asp:Panel ID="pnlAbrirCaja" runat="server" CssClass="card mb-4">
        <div class="card-header">
            <h5 class="mb-0">Abrir caja</h5>
        </div>
        <div class="card-body">
            <div class="row g-2 align-items-end">
                <div class="col-md-3">
                    <label class="form-label">Monto inicial ($)</label>
                    <asp:TextBox ID="txtMontoApertura" runat="server" CssClass="form-control" ValidationGroup="AbrirCaja" placeholder="0,00" />
                    <asp:RequiredFieldValidator ID="rfvMontoApertura" runat="server" ControlToValidate="txtMontoApertura" ErrorMessage="*" CssClass="text-danger" ValidationGroup="AbrirCaja" Display="Dynamic" />
                    <asp:RangeValidator ID="rvMontoApertura" runat="server" ControlToValidate="txtMontoApertura" MinimumValue="0" MaximumValue="99999999" Type="Double" ErrorMessage="Inválido" CssClass="text-danger" ValidationGroup="AbrirCaja" Display="Dynamic" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnAbrirCaja" runat="server" Text="Abrir caja" CssClass="btn btn-primary" OnClick="btnAbrirCaja_Click" ValidationGroup="AbrirCaja" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlCajaAbierta" runat="server" CssClass="card mb-4" Visible="false">
        <div class="card-header">
            <h5 class="mb-0">Caja abierta</h5>
        </div>
        <div class="card-body">
            <p class="mb-1"><strong>Apertura:</strong> <asp:Literal ID="litFechaApertura" runat="server" /> — <asp:Literal ID="litMontoApertura" runat="server" /></p>
            <p class="mb-3"><strong>Efectivo cobrado hasta el momento:</strong> <asp:Literal ID="litEfectivoAcumulado" runat="server" /></p>
            <hr />
            <div class="row g-2 align-items-end">
                <div class="col-md-3">
                    <label class="form-label">Monto contado al cierre ($)</label>
                    <asp:TextBox ID="txtMontoCierre" runat="server" CssClass="form-control" ValidationGroup="CerrarCaja" placeholder="0,00" />
                    <asp:RequiredFieldValidator ID="rfvMontoCierre" runat="server" ControlToValidate="txtMontoCierre" ErrorMessage="*" CssClass="text-danger" ValidationGroup="CerrarCaja" Display="Dynamic" />
                    <asp:RangeValidator ID="rvMontoCierre" runat="server" ControlToValidate="txtMontoCierre" MinimumValue="0" MaximumValue="99999999" Type="Double" ErrorMessage="Inválido" CssClass="text-danger" ValidationGroup="CerrarCaja" Display="Dynamic" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnCerrarCaja" runat="server" Text="Cerrar caja" CssClass="btn btn-primary" OnClick="btnCerrarCaja_Click" ValidationGroup="CerrarCaja" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <div class="card">
        <div class="card-header">
            <h5 class="mb-0">Histórico de cajas</h5>
        </div>
        <div class="card-body p-0">
            <asp:GridView ID="dgvHistorico" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm mb-0" EmptyDataText="Sin cajas registradas.">
                <Columns>
                    <asp:BoundField DataField="FechaApertura"        HeaderText="Apertura"       DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="UsuarioApertura"      HeaderText="Abrió" />
                    <asp:BoundField DataField="MontoApertura"        HeaderText="Monto apertura"  DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    <asp:BoundField DataField="FechaCierre"          HeaderText="Cierre"         DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="UsuarioCierre"        HeaderText="Cerró" />
                    <asp:BoundField DataField="MontoCierreDeclarado" HeaderText="Declarado"      DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    <asp:BoundField DataField="MontoCierreCalculado" HeaderText="Calculado"      DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    <asp:BoundField DataField="Diferencia"           HeaderText="Diferencia"     DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
```

- [ ] **Step 2: Write `Caja.aspx.cs`**

```csharp
using System;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class Caja : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u = Session["Usuario"] as Usuario;
            if (!Seguridad.SesionActiva(u) || (!Seguridad.EsAdmin(u) && !Seguridad.EsVendedor(u)))
            {
                Response.Redirect("Default.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                CargarEstado();
            }
        }

        private void CargarEstado()
        {
            Dominio.Caja cajaAbierta = new CajaNegocio().ObtenerCajaAbierta();

            pnlAbrirCaja.Visible = (cajaAbierta == null);
            pnlCajaAbierta.Visible = (cajaAbierta != null);

            if (cajaAbierta != null)
            {
                litFechaApertura.Text = cajaAbierta.FechaApertura.ToString("dd/MM/yyyy HH:mm");
                litMontoApertura.Text = cajaAbierta.MontoApertura.ToString("C");
                litEfectivoAcumulado.Text = new PagoNegocio().SumarEfectivoDesde(cajaAbierta.FechaApertura).ToString("C");
            }

            dgvHistorico.DataSource = new CajaNegocio().Listar();
            dgvHistorico.DataBind();
        }

        protected void btnAbrirCaja_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMontoApertura.Text, out decimal monto) || monto < 0)
            {
                MostrarMensaje("Ingrese un monto inicial válido.", false);
                return;
            }

            try
            {
                Usuario u = (Usuario)Session["Usuario"];
                new CajaNegocio().AbrirCaja(monto, u.Id);
                txtMontoApertura.Text = "";
                MostrarMensaje("Caja abierta correctamente.", true);
                CargarEstado();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, false);
            }
        }

        protected void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMontoCierre.Text, out decimal monto) || monto < 0)
            {
                MostrarMensaje("Ingrese un monto de cierre válido.", false);
                return;
            }

            try
            {
                Dominio.Caja cajaAbierta = new CajaNegocio().ObtenerCajaAbierta();
                if (cajaAbierta == null)
                {
                    MostrarMensaje("No hay una caja abierta para cerrar.", false);
                    return;
                }

                Usuario u = (Usuario)Session["Usuario"];
                new CajaNegocio().CerrarCaja(cajaAbierta.Id, monto, u.Id);

                Dominio.Caja cajaCerrada = new CajaNegocio().Listar().FirstOrDefault(c => c.Id == cajaAbierta.Id);
                decimal diferencia = (cajaCerrada != null && cajaCerrada.Diferencia.HasValue) ? cajaCerrada.Diferencia.Value : 0m;

                txtMontoCierre.Text = "";
                MostrarMensaje("Caja cerrada. Diferencia de arqueo: " + diferencia.ToString("C"), true);
                CargarEstado();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, false);
            }
        }

        private void MostrarMensaje(string mensaje, bool exito)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = exito ? "alert alert-success w-100 mb-3" : "alert alert-danger w-100 mb-3";
        }
    }
}
```

- [ ] **Step 3: Write `Caja.aspx.designer.cs`**

```csharp
namespace AplicacionWebComercio
{
    public partial class Caja
    {
        protected global::System.Web.UI.WebControls.Label lblMensaje;
        protected global::System.Web.UI.WebControls.Panel pnlAbrirCaja;
        protected global::System.Web.UI.WebControls.TextBox txtMontoApertura;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvMontoApertura;
        protected global::System.Web.UI.WebControls.RangeValidator rvMontoApertura;
        protected global::System.Web.UI.WebControls.Button btnAbrirCaja;
        protected global::System.Web.UI.WebControls.Panel pnlCajaAbierta;
        protected global::System.Web.UI.Literal litFechaApertura;
        protected global::System.Web.UI.Literal litMontoApertura;
        protected global::System.Web.UI.Literal litEfectivoAcumulado;
        protected global::System.Web.UI.WebControls.TextBox txtMontoCierre;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvMontoCierre;
        protected global::System.Web.UI.WebControls.RangeValidator rvMontoCierre;
        protected global::System.Web.UI.WebControls.Button btnCerrarCaja;
        protected global::System.Web.UI.WebControls.GridView dgvHistorico;
    }
}
```

- [ ] **Step 4: Register the new files in `AplicacionWebComercio.csproj`**

Add `<Content Include="Caja.aspx" />` and:

```xml
    <Compile Include="Caja.aspx.cs">
      <DependentUpon>Caja.aspx</DependentUpon>
      <SubType>ASPXCodeBehind</SubType>
    </Compile>
    <Compile Include="Caja.aspx.designer.cs">
      <DependentUpon>Caja.aspx</DependentUpon>
    </Compile>
```

- [ ] **Step 5: Add the nav entry in `Site.Master`**

Inside the "Movimientos" dropdown, add `liCaja` between `liVentas` and `liTrazabilidad`:

```aspx
                                <li runat="server" id="liVentas">
                                    <a class="dropdown-item" href="Ventas.aspx">Ventas</a>
                                </li>
                                <li runat="server" id="liCaja">
                                    <a class="dropdown-item" href="Caja.aspx">Caja</a>
                                </li>
                                <li runat="server" id="liTrazabilidad">
```

- [ ] **Step 6: Update `Site.Master.cs`**

```csharp
            // Grupo Movimientos (Admin + Vendedor); Trazabilidad solo Admin
            liGrupoMovimientos.Visible = esAdmin || esVend;
            liCompras.Visible          = esAdmin || esVend;
            liVentas.Visible           = esAdmin || esVend;
            liCaja.Visible             = esAdmin || esVend;
            liTrazabilidad.Visible     = esAdmin;
```

- [ ] **Step 7: Add the designer field to `Site.Master.designer.cs`**

```csharp
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl liCaja;
```

- [ ] **Step 8: Build to verify it compiles**

Run (see Global Constraints — `dotnet build` cannot build this project):
```bash
MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
```
Expected: no `error` lines; last output lines show each project's `.dll` output path (e.g. `AplicacionWebComercio -> ...\bin\AplicacionWebComercio.dll`).

- [ ] **Step 9: Verify the anonymous-access guard, then the rest via SQL**

`Caja.aspx` requires a logged-in session (`Session["Usuario"]`), which `WebFetch` cannot establish (no login form submission, no cookie jar). What IS checkable: start IIS Express per Global Constraints and `WebFetch` on `http://localhost:59928/Caja.aspx` with no session — confirm it redirects (no session means `Seguridad.SesionActiva` is false regardless of role, so this exercises the guard clause, just not the Admin/Vendedor-vs-Cliente distinction). Then verify the underlying business logic directly, reusing the same SQL pattern as Task 6 Step 3 (abrir con $1000, EXEC `SP_Pagos_Crear` with `Tipo='Efectivo', Monto=1000` dated after the apertura, cerrar with `MontoCierreDeclarado=1000`, confirm `SP_Caja_Listar` shows `MontoCierreCalculado=1000` and `Diferencia=0`), then clean up the rows. Leave in the report: "logging in as Admin/Vendedor vs Cliente to confirm the nav entry and the full open-caja to sell to close-caja flow through the UI needs manual QA by the user."

- [ ] **Step 10: Commit**

```bash
git add TPC-Comercio/AplicacionWebComercio/Caja.aspx TPC-Comercio/AplicacionWebComercio/Caja.aspx.cs TPC-Comercio/AplicacionWebComercio/Caja.aspx.designer.cs TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj TPC-Comercio/AplicacionWebComercio/Site.Master TPC-Comercio/AplicacionWebComercio/Site.Master.cs TPC-Comercio/AplicacionWebComercio/Site.Master.designer.cs
git commit -m "feat: pagina de Caja (apertura/cierre/arqueo) y entrada de nav"
```

---

### Task 10: 3.6 — `site-ui.js` (toast automático + loader de postback)

**Files:**
- Create: `TPC-Comercio/AplicacionWebComercio/Scripts/site-ui.js`
- Modify: `TPC-Comercio/AplicacionWebComercio/Site.Master`
- Modify: `TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj`

**Interfaces:**
- Consumes: Bootstrap's `bootstrap.Toast` (already loaded via `bootstrap.bundle.js`), the page's `#MainContent` content placeholder, `window.__doPostBack` (emitted by ASP.NET WebForms itself — always present on pages with server controls that trigger postback).
- Produces: sitewide auto-toast conversion of any visible `.alert` inside the content area, and a full-page loading overlay on every postback (button click or `__doPostBack`-driven link/dropdown) — **no changes needed to any existing page's code-behind**, since every page here uses full postbacks (confirmed: no `UpdatePanel` usage anywhere in the project) and already renders its feedback as a `.alert` element with the existing `MostrarMensaje`/`lblMensaje` pattern.

- [ ] **Step 1: Write `site-ui.js`**

```javascript
// TPC-Comercio/AplicacionWebComercio/Scripts/site-ui.js
(function () {
    "use strict";

    function crearToastContainer() {
        var existente = document.getElementById("toastContainer");
        if (existente) return existente;

        var contenedor = document.createElement("div");
        contenedor.id = "toastContainer";
        contenedor.className = "toast-container position-fixed top-0 end-0 p-3";
        contenedor.style.zIndex = "1080";
        document.body.appendChild(contenedor);
        return contenedor;
    }

    function convertirAlertasEnToasts() {
        var contenido = document.getElementById("MainContent") || document.body;
        var alertas = contenido.querySelectorAll(".alert:not(.d-none)");

        if (alertas.length === 0) return;

        var contenedor = crearToastContainer();

        alertas.forEach(function (alerta) {
            var texto = alerta.textContent.trim();
            if (!texto) return;

            var esError = alerta.classList.contains("alert-danger");

            var toastEl = document.createElement("div");
            toastEl.className = "toast align-items-center border-0 " + (esError ? "text-bg-danger" : "text-bg-success");
            toastEl.setAttribute("role", "alert");
            toastEl.innerHTML =
                '<div class="d-flex">' +
                '<div class="toast-body">' + texto + '</div>' +
                '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>' +
                '</div>';

            contenedor.appendChild(toastEl);
            var toast = new bootstrap.Toast(toastEl, { delay: 4000 });
            toast.show();

            alerta.classList.add("d-none");
        });
    }

    function mostrarLoaderOverlay() {
        if (document.getElementById("loaderOverlay")) return;

        var overlay = document.createElement("div");
        overlay.id = "loaderOverlay";
        overlay.style.cssText =
            "position:fixed;inset:0;background:rgba(255,255,255,0.6);" +
            "display:flex;align-items:center;justify-content:center;z-index:1090;";
        overlay.innerHTML = '<div class="spinner-border text-primary" role="status"></div>';
        document.body.appendChild(overlay);
    }

    function habilitarLoaderEnPostback() {
        var formulario = document.getElementById("form1");
        if (!formulario) return;

        formulario.addEventListener("submit", mostrarLoaderOverlay);

        if (typeof window.__doPostBack === "function") {
            var doPostBackOriginal = window.__doPostBack;
            window.__doPostBack = function (eventTarget, eventArgument) {
                mostrarLoaderOverlay();
                doPostBackOriginal(eventTarget, eventArgument);
            };
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        convertirAlertasEnToasts();
        habilitarLoaderEnPostback();
    });
})();
```

- [ ] **Step 2: Include it in `Site.Master`, right after the bootstrap bundle**

```aspx
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/Scripts/bootstrap.bundle.js") %>
        <%: Scripts.Render("~/Scripts/site-ui.js") %>
    </asp:PlaceHolder>
```

- [ ] **Step 3: Register the file in `AplicacionWebComercio.csproj`**

Add `<Content Include="Scripts\site-ui.js" />` next to `Scripts\bootstrap.bundle.js`.

- [ ] **Step 4: Build to verify it compiles**

Run (see Global Constraints — `dotnet build` cannot build this project):
```bash
MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
```
Expected: no `error` lines; last output lines show each project's `.dll` output path (e.g. `AplicacionWebComercio -> ...\bin\AplicacionWebComercio.dll`).

- [ ] **Step 5: Verify the script is wired up (the interactive behavior itself needs a real browser)**

`WebFetch` only does a GET and converts HTML/JS to text — it does not execute JavaScript, so it cannot observe a toast appearing or a spinner overlay. What IS checkable: start IIS Express per Global Constraints, `WebFetch` on `http://localhost:59928/Scripts/site-ui.js` and confirm it serves the file's actual content (not a 404), then `WebFetch` on `http://localhost:59928/ComprasFormulario.aspx` and confirm the rendered HTML includes a `<script src="...site-ui.js">` (or similarly-rendered `Scripts.Render` output) tag. Leave in the report: "the actual interactive behavior — toast replacing the inline alert on a validation error, spinner overlay on submit and on a LinkButton-driven postback like 'Quitar' — needs manual QA by the user in a real browser."

- [ ] **Step 6: Commit**

```bash
git add TPC-Comercio/AplicacionWebComercio/Scripts/site-ui.js TPC-Comercio/AplicacionWebComercio/Site.Master TPC-Comercio/AplicacionWebComercio/AplicacionWebComercio.csproj
git commit -m "feat: toast automatico y loader de postback compartidos (site-ui.js)"
```

---

### Task 11: 3.6 — extraer estilos inline de `FacturaReporte.aspx` y `CompraReporte.aspx`

**Files:**
- Modify: `TPC-Comercio/AplicacionWebComercio/Content/Site.css`
- Modify: `TPC-Comercio/AplicacionWebComercio/FacturaReporte.aspx`
- Modify: `TPC-Comercio/AplicacionWebComercio/CompraReporte.aspx`

**Interfaces:** none — pure CSS refactor, no C#/markup control changes beyond class names.

**Important:** these two pages don't use `Site.Master`, so their existing inline `<style>` rules use **unscoped selectors** (bare `body`, `table`, `th, td`, `.btn`, `.btn-secondary`). Moving them verbatim into the shared `Site.css` (used by every `Site.Master` page, i.e. the whole rest of the site) would leak `.btn`/`table` overrides sitewide and break Bootstrap styling everywhere else. Scope every rule under a new `.comprobante-container` class and rename the button classes to avoid colliding with Bootstrap's own `.btn`.

- [ ] **Step 1: Append the scoped rules to `Content/Site.css`**

```css

/* Comprobantes de impresion (Factura/Compra) - FacturaReporte.aspx y CompraReporte.aspx */
.comprobante-container {
    max-width: 800px;
    margin: 0 auto;
    padding: 30px;
    border: 1px solid #ddd;
    box-shadow: 0 0 10px rgba(0,0,0,0.1);
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    color: #333;
}

.comprobante-container .header {
    text-align: center;
    border-bottom: 2px solid #333;
    padding-bottom: 20px;
    margin-bottom: 20px;
}

.comprobante-container .header h1 {
    margin: 0;
    font-size: 28px;
    text-transform: uppercase;
    letter-spacing: 2px;
}

.comprobante-container .header .subtitulo {
    margin: 4px 0;
    color: #777;
}

.comprobante-container .header .numero {
    font-size: 16px;
    font-weight: bold;
    margin-top: 8px;
}

.comprobante-container .info-section {
    display: flex;
    justify-content: space-between;
    margin-bottom: 30px;
}

.comprobante-container .info-box {
    width: 48%;
}

.comprobante-container .info-box h4 {
    margin-top: 0;
    border-bottom: 1px solid #eee;
    padding-bottom: 5px;
}

.comprobante-container .info-box p {
    margin: 4px 0;
    font-size: 14px;
}

.comprobante-container table {
    width: 100%;
    border-collapse: collapse;
    margin-bottom: 30px;
}

.comprobante-container th,
.comprobante-container td {
    border: 1px solid #ddd;
    padding: 10px 12px;
    text-align: left;
}

.comprobante-container th {
    background-color: #f8f9fa;
    font-weight: bold;
}

.comprobante-container td.num {
    text-align: right;
}

.comprobante-container .totales {
    text-align: right;
    font-size: 18px;
    border-top: 2px solid #333;
    padding-top: 10px;
}

.comprobante-container .totales strong {
    font-size: 24px;
}

.comprobante-container .no-print {
    text-align: center;
    margin-top: 24px;
}

.comprobante-container .btn-comprobante {
    padding: 10px 20px;
    background-color: #007bff;
    color: white;
    border: none;
    cursor: pointer;
    font-size: 15px;
    border-radius: 5px;
    text-decoration: none;
    display: inline-block;
}

.comprobante-container .btn-comprobante:hover {
    background-color: #0056b3;
}

.comprobante-container .btn-comprobante-secondary {
    background-color: #6c757d;
    margin-left: 10px;
}

@media print {
    .comprobante-container .no-print {
        display: none;
    }
    .comprobante-container {
        border: none;
        box-shadow: none;
        padding: 0;
    }
}
```

- [ ] **Step 2: Update `FacturaReporte.aspx`**

Remove the entire inline `<style>...</style>` block from `<head>` and add a stylesheet link in its place:

```aspx
    <link href="Content/Site.css" rel="stylesheet" type="text/css" />
```

Then rename classes in the body: `class="factura-container"` → `class="comprobante-container"`; `class="btn"` (Imprimir button) → `class="btn-comprobante"`; `class="btn btn-secondary"` (Volver link) → `class="btn-comprobante btn-comprobante-secondary"`. Leave every other markup, control, and code-behind untouched.

- [ ] **Step 3: Update `CompraReporte.aspx`**

Same changes: remove its inline `<style>` block, add `<link href="Content/Site.css" rel="stylesheet" type="text/css" />`, rename `class="compra-container"` → `class="comprobante-container"`, `class="btn"` → `class="btn-comprobante"`, `class="btn btn-secondary"` → `class="btn-comprobante btn-comprobante-secondary"`.

- [ ] **Step 4: Build to verify it compiles**

Run (see Global Constraints — `dotnet build` cannot build this project):
```bash
MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
```
Expected: no `error` lines; last output lines show each project's `.dll` output path (e.g. `AplicacionWebComercio -> ...\bin\AplicacionWebComercio.dll`).

- [ ] **Step 5: Verify structurally via WebFetch; leave the visual/print check for the user**

Start IIS Express per Global Constraints. `WebFetch` on `http://localhost:59928/Content/Site.css` and confirm it contains the new `.comprobante-container` rules. `WebFetch` on `http://localhost:59928/FacturaReporte.aspx?id=1` and `http://localhost:59928/CompraReporte.aspx?id=1` and confirm each: has no inline `<style>` block anymore, has a `<link ... Content/Site.css ...>` tag, uses `class="comprobante-container"` and `class="btn-comprobante..."`, and shows the expected data (factura/compra number, items, total) with no server error text. `WebFetch` cannot render CSS or compare pixels, so it cannot confirm the pages still look the same, or that `Ventas.aspx`'s Bootstrap buttons/tables are visually unaffected — leave in the report: "visual comparison (both comprobante pages look identical to before, print preview via Ctrl+P, and no regression on Bootstrap-styled pages like Ventas.aspx) needs manual QA by the user."

- [ ] **Step 6: Commit**

```bash
git add TPC-Comercio/AplicacionWebComercio/Content/Site.css TPC-Comercio/AplicacionWebComercio/FacturaReporte.aspx TPC-Comercio/AplicacionWebComercio/CompraReporte.aspx
git commit -m "refactor: mover estilos inline de comprobantes a Site.css con clases scoped"
```

---

### Task 12: Eliminar el proyecto temporal `Negocio.Tests`

**Files:**
- Delete: `TPC-Comercio/Negocio.Tests/` (entire directory)

**Interfaces:** none.

- [ ] **Step 1: Confirm all prior tasks are done and committed**

Run: `git log --oneline -12` and confirm commits for Tasks 1–11 are present.

- [ ] **Step 2: Remove the temporary test project**

```bash
git rm -r TPC-Comercio/Negocio.Tests
```

- [ ] **Step 3: Verify the rest of the solution still builds without it**

Run:
```bash
dotnet build TPC-Comercio/Dominio/Dominio.csproj
dotnet build TPC-Comercio/AccesoDatos/AccesoDatos.csproj
dotnet build TPC-Comercio/Negocio/Negocio.csproj
MSYS_NO_PATHCONV=1 "/c/Program Files/Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/amd64/MSBuild.exe" "TPC-Comercio/TPC-Comercio.slnx" -p:Configuration=Debug -nologo -v:minimal
```
Expected: the first three each print `Compilación correcta. 0 Errores`; the last (which builds all four projects, including `AplicacionWebComercio` — see Global Constraints for why it needs full MSBuild, not `dotnet build`) prints no `error` lines (none of the four ever referenced `Negocio.Tests`, only the reverse).

- [ ] **Step 4: Commit**

```bash
git commit -m "chore: remove temporary Negocio.Tests project"
```

---

## Self-Review Notes

- **Spec coverage:** 3.1 → Task 8. 3.4 → Tasks 3–4. 3.5 → Tasks 5–9. 3.6 → Tasks 10–11. Testing strategy (agreed deviation from a bare "TDD everything") → Tasks 1, 2, 12. All spec sections have a task.
- **Placeholder scan:** no TBD/TODO; every step has literal file paths, full code, and concrete expected command output.
- **Type consistency:** `Cuota` (NroCuota, Monto, Interes, Vencimiento) is introduced in Task 2 and used identically in Tasks 3, 5, 8. `Caja` (with `UsuarioApertura`/`UsuarioCierre`/`Diferencia`) is introduced in Task 4 and consumed identically in Tasks 6 and 9. `PagoNegocio.RegistrarPago(int, decimal, string, int)` signature matches between Task 5's definition and Task 7's call site. `CajaNegocio` method names (`AbrirCaja`, `ObtenerCajaAbierta`, `CerrarCaja`, `Listar`) match between Task 6 and Task 9.
- **Regression risk caught during planning:** moving the print pages' inline CSS into the shared `Site.css` verbatim would have leaked unscoped `table`/`.btn` rules sitewide — fixed by scoping everything under `.comprobante-container` and renaming button classes (Task 11).
- **Tooling validated live** (not just assumed) before writing the plan: `dotnet build` on the existing classic `Negocio.csproj` succeeds, and an SDK-style `net48` MSTest project referencing it via `ProjectReference` builds and runs with `dotnet test` — confirmed by an actual scratch run in this session.
