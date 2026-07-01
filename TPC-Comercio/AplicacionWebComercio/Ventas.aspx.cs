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

                if (Negocio.Seguridad.EsCliente(u))
                {
                    dgvVentas.DataSource = negocio.Listar(u.IdEntidad);
                }
                else
                {
                    dgvVentas.DataSource = negocio.Listar();
                }
                dgvVentas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar las ventas: " + System.Web.HttpUtility.HtmlEncode(ex.Message);
                lblMensaje.Visible = true;
            }
        }
    }
}
