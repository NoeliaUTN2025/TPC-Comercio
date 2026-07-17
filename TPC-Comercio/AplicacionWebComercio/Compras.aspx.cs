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
                Dominio.FiltrosBusqueda filtros = ctrlFiltros.ObtenerFiltros();
                var lista = new CompraNegocio().Listar(filtros);

                dgvCompras.DataSource = lista;
                dgvCompras.DataBind();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar las compras: " + System.Web.HttpUtility.HtmlEncode(ex.Message);
                lblMensaje.Visible = true;
            }
        }

        protected void ctrlFiltros_Filtrar(object sender, EventArgs e)
        {
            CargarGrilla();
        }
    }
}
