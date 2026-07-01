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
            liLogin.Visible      = !activo;
            liCrearCuenta.Visible = !activo;

            // Solo autenticados
            liLogout.Visible            = activo;
            liCambiarContrasena.Visible = activo;

            // Módulos Admin + Vendedor
            liClientes.Visible   = esAdmin || esVend;
            liProveedores.Visible = esAdmin || esVend;
            liProductos.Visible  = esAdmin || esVend;
            liCompras.Visible    = esAdmin || esVend;
            liVentas.Visible     = esAdmin || esVend;

            // Solo Admin
            liMarcas.Visible       = esAdmin;
            liCategorias.Visible   = esAdmin;
            liUsuarios.Visible     = esAdmin;
            liTrazabilidad.Visible = esAdmin;

            // Solo Cliente
            liCatalogo.Visible = esCli;
            liVentasCliente.Visible = esCli;

            // Solo Proveedor
            liMisCompras.Visible = esProv;
        }
    }
}
