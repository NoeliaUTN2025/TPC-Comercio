using System;
using System.Collections.Generic;
using System.Web.UI;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class BarraFiltros : System.Web.UI.UserControl
    {
        public event EventHandler Filtrar;

        public bool MostrarCategoria
        {
            get { return pnlCategoria.Visible; }
            set { pnlCategoria.Visible = value; }
        }

        public bool MostrarMarca
        {
            get { return pnlMarca.Visible; }
            set { pnlMarca.Visible = value; }
        }

        public bool MostrarFechas
        {
            get { return pnlFechas.Visible; }
            set { pnlFechas.Visible = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (MostrarCategoria) CargarCategorias();
                if (MostrarMarca) CargarMarcas();
            }
        }

        private void CargarCategorias()
        {
            ddlCategoria.DataSource = new CategoriaNegocio().Listar();
            ddlCategoria.DataTextField = "Descripcion";
            ddlCategoria.DataValueField = "Id";
            ddlCategoria.DataBind();
            ddlCategoria.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todas", "0"));
        }

        private void CargarMarcas()
        {
            ddlMarca.DataSource = new MarcaNegocio().Listar();
            ddlMarca.DataTextField = "Descripcion";
            ddlMarca.DataValueField = "Id";
            ddlMarca.DataBind();
            ddlMarca.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todas", "0"));
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            Filtrar?.Invoke(this, EventArgs.Empty);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtTexto.Text = "";
            if (MostrarCategoria && ddlCategoria.Items.Count > 0) ddlCategoria.SelectedIndex = 0;
            if (MostrarMarca && ddlMarca.Items.Count > 0) ddlMarca.SelectedIndex = 0;
            if (MostrarFechas)
            {
                txtFechaDesde.Text = "";
                txtFechaHasta.Text = "";
            }
            Filtrar?.Invoke(this, EventArgs.Empty);
        }

        public FiltrosBusqueda ObtenerFiltros()
        {
            FiltrosBusqueda filtros = new FiltrosBusqueda();
            filtros.Texto = string.IsNullOrWhiteSpace(txtTexto.Text) ? null : txtTexto.Text.Trim();
            
            if (MostrarCategoria && ddlCategoria.SelectedIndex > 0)
                filtros.IdCategoria = int.Parse(ddlCategoria.SelectedValue);

            if (MostrarMarca && ddlMarca.SelectedIndex > 0)
                filtros.IdMarca = int.Parse(ddlMarca.SelectedValue);

            if (MostrarFechas)
            {
                if (DateTime.TryParse(txtFechaDesde.Text, out DateTime fd)) filtros.FechaDesde = fd;
                if (DateTime.TryParse(txtFechaHasta.Text, out DateTime fh)) filtros.FechaHasta = fh;
            }

            return filtros;
        }
    }
}
