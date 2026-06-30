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
            dgvCompras.DataSource = new CompraNegocio().Listar();
            dgvCompras.DataBind();
        }
    }
}
