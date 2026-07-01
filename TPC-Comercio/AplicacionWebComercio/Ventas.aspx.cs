using System;
using System.Web.UI.WebControls;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class Ventas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                dgvVentas.DataSource = negocio.Listar();
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
