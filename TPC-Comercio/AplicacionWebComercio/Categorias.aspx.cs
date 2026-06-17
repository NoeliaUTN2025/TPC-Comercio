using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class Categorias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvCategorias.DataSource = new CategoriaNegocio().Listar();
            dgvCategorias.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Categoria c = new Categoria { Descripcion = txtDescripcion.Text.Trim() };
            CategoriaNegocio negocio = new CategoriaNegocio();

            if (hfId.Value == "0")
                negocio.Agregar(c);
            else
            {
                c.Id = int.Parse(hfId.Value);
                negocio.Modificar(c);
            }

            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvCategorias.EditIndex = -1;
            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void dgvCategorias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Categoria c = new CategoriaNegocio().Listar().Find(x => x.Id == id);

            hfId.Value = c.Id.ToString();
            txtDescripcion.Text = c.Descripcion;

            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvCategorias_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = (int)dgvCategorias.DataKeys[e.RowIndex].Value;
            new CategoriaNegocio().Eliminar(id);
            CargarGrilla();
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "0";
            txtDescripcion.Text = "";
        }
    }
}
