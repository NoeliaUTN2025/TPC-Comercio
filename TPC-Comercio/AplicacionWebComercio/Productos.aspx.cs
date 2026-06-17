using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class Productos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvProductos.DataSource = new ProductoNegocio().Listar();
            dgvProductos.DataBind();
        }

        private void CargarDropDowns()
        {
            ddlMarca.DataSource = new MarcaNegocio().Listar();
            ddlMarca.DataBind();

            ddlCategoria.DataSource = new CategoriaNegocio().Listar();
            ddlCategoria.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            CargarDropDowns();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Producto p = new Producto
            {
                Codigo = txtCodigo.Text.Trim(),
                NombreProducto = txtNombre.Text.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                Precio = decimal.Parse(txtPrecio.Text),
                StockMinimo = int.Parse(txtStockMinimo.Text),
                PorcentajeGanancia = decimal.Parse(txtPorcentajeGanancia.Text),
                marca = new Marca { Id = int.Parse(ddlMarca.SelectedValue) },
                categoria = new Categoria { Id = int.Parse(ddlCategoria.SelectedValue) }
            };

            ProductoNegocio negocio = new ProductoNegocio();

            if (hfId.Value == "0")
                negocio.Agregar(p);
            else
            {
                p.Id = int.Parse(hfId.Value);
                negocio.Modificar(p);
            }

            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvProductos.EditIndex = -1;
            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void dgvProductos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Producto p = new ProductoNegocio().Listar().Find(x => x.Id == id);

            hfId.Value = p.Id.ToString();
            txtCodigo.Text = p.Codigo;
            txtNombre.Text = p.NombreProducto;
            txtDescripcion.Text = p.Descripcion;
            txtPrecio.Text = p.Precio.ToString();
            txtStockMinimo.Text = p.StockMinimo.ToString();
            txtPorcentajeGanancia.Text = p.PorcentajeGanancia.ToString();

            CargarDropDowns();
            ddlMarca.SelectedValue = p.marca.Id.ToString();
            ddlCategoria.SelectedValue = p.categoria.Id.ToString();

            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = (int)dgvProductos.DataKeys[e.RowIndex].Value;
            new ProductoNegocio().EliminarLogico(id);
            CargarGrilla();
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "0";
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtStockMinimo.Text = "";
            txtPorcentajeGanancia.Text = "";
        }
    }
}
