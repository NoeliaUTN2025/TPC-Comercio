using System;
using System.Web.UI;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u     = Session["Usuario"] as Usuario;
            bool activo   = Seguridad.SesionActiva(u);
            bool esAdmin  = activo && Seguridad.EsAdmin(u);
            bool esVend   = activo && Seguridad.EsVendedor(u);
            bool esCli    = activo && Seguridad.EsCliente(u);
            bool esProv   = activo && Seguridad.EsProveedor(u);

            // Siempre visible
            liInicio.Visible = true;

            // Solo anónimos
            liLogin.Visible       = !activo;
            liCrearCuenta.Visible = !activo;

            // Solo autenticados
            liLogout.Visible            = activo;
            liCambiarContrasena.Visible = activo;

            // Grupo Inventario (Admin + Vendedor); Marcas y Categorías solo Admin
            liGrupoInventario.Visible = esAdmin || esVend;
            liProductos.Visible       = esAdmin || esVend;
            liMarcas.Visible          = esAdmin;
            liCategorias.Visible      = esAdmin;

            // Grupo Movimientos (Admin + Vendedor); Trazabilidad solo Admin
            liGrupoMovimientos.Visible = esAdmin || esVend;
            liCompras.Visible          = esAdmin || esVend;
            liVentas.Visible           = esAdmin || esVend;
            liCaja.Visible             = esAdmin || esVend;
            liTrazabilidad.Visible     = esAdmin;
            liReportes.Visible         = esAdmin;

            // Grupo Personas (Admin + Vendedor); Usuarios solo Admin
            liGrupoPersonas.Visible = esAdmin || esVend;
            liClientes.Visible      = esAdmin || esVend;
            liProveedores.Visible   = esAdmin || esVend;
            liUsuarios.Visible      = esAdmin;

            // Solo Cliente
            liCatalogo.Visible      = esCli;
            liVentasCliente.Visible = esCli;

            // Solo Proveedor
            liMisCompras.Visible = esProv;
        }
    }
}
