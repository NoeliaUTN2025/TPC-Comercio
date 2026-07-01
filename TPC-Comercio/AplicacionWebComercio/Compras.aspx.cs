using System;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class Compras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            try
            {
                dgvCompras.DataSource = new CompraNegocio().Listar();
                dgvCompras.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar las compras: " + System.Web.HttpUtility.HtmlEncode(ex.Message);
                lblMensaje.Visible = true;
            }
        }
    }
}
