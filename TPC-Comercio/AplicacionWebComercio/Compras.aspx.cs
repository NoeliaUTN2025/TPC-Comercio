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
                var lista = new CompraNegocio().Listar();
                Dominio.FiltrosBusqueda filtros = ctrlFiltros.ObtenerFiltros();

                if (!string.IsNullOrEmpty(filtros.Texto))
                    lista = lista.FindAll(x => x.Proveedor.RazonSocial.ToLower().Contains(filtros.Texto.ToLower()));

                if (filtros.FechaDesde.HasValue)
                    lista = lista.FindAll(x => x.Fecha.Date >= filtros.FechaDesde.Value.Date);

                if (filtros.FechaHasta.HasValue)
                    lista = lista.FindAll(x => x.Fecha.Date <= filtros.FechaHasta.Value.Date);

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
