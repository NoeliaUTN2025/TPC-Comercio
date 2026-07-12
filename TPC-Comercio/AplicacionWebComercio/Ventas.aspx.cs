using System;
using System.Web.UI.WebControls;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class Ventas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Valida si el usuario tiene sesi�n activa y si es administrador, vendedor o cliente
            Dominio.Usuario u = Session["Usuario"] as Dominio.Usuario;
            if (!Negocio.Seguridad.SesionActiva(u) || (!Negocio.Seguridad.EsAdmin(u) && !Negocio.Seguridad.EsVendedor(u) && !Negocio.Seguridad.EsCliente(u)))
            {
                Response.Redirect("Default.aspx", false);
                return;
            }

            if (Negocio.Seguridad.EsCliente(u))
            {
                litTitulo.Text = "Mis Compras";
                lnkNuevaVenta.Visible = false;
            }

            if (!IsPostBack)
            {
                CargarVentas();
            }
        }

        private void CargarVentas()
        {
            try
            {
                FacturaNegocio negocio = new FacturaNegocio();
                Dominio.Usuario u = Session["Usuario"] as Dominio.Usuario;

                var lista = Negocio.Seguridad.EsCliente(u) ? negocio.Listar(u.IdEntidad) : negocio.Listar();
                
                Dominio.FiltrosBusqueda filtros = ctrlFiltros.ObtenerFiltros();

                if (!string.IsNullOrEmpty(filtros.Texto))
                {
                    string txt = filtros.Texto.ToLower();
                    lista = lista.FindAll(x => 
                        (x.Cliente != null && (x.Cliente.Nombre.ToLower().Contains(txt) || x.Cliente.Apellido.ToLower().Contains(txt))) || 
                        x.NumeroFactura.ToString().Contains(txt));
                }

                if (filtros.FechaDesde.HasValue)
                    lista = lista.FindAll(x => x.Fecha.Date >= filtros.FechaDesde.Value.Date);

                if (filtros.FechaHasta.HasValue)
                    lista = lista.FindAll(x => x.Fecha.Date <= filtros.FechaHasta.Value.Date);

                dgvVentas.DataSource = lista;
                dgvVentas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar las ventas: " + System.Web.HttpUtility.HtmlEncode(ex.Message);
                lblMensaje.Visible = true;
            }
        }

        protected void ctrlFiltros_Filtrar(object sender, EventArgs e)
        {
            CargarVentas();
        }
    }
}
