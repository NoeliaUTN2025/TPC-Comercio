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
            if (!Seguridad.SesionActiva((Session)["Usuario"]))
            {
                Response.Redirect("Login.aspx", false);
                return;
            }

            Usuario usuario = (Usuario)Session["Usuario"];
            if (!(Seguridad.EsAdmin(usuario) || Seguridad.EsVendedor(usuario)))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No tiene permisos para acceder a esta sección.'); window.location='Default.aspx';", true);
                return;
            }

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
            try
            {
                int id = (int)dgvCategorias.DataKeys[e.RowIndex].Value;
                new CategoriaNegocio().Eliminar(id);
                lblError.Visible = false;
            }
            catch (Exception ex)
            {
                // Mostramos el mensaje de negocio al usuario (sin exponer stack trace)
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
            finally
            {
                CargarGrilla();
            }
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "0";
            txtDescripcion.Text = "";
        }
    }
}
