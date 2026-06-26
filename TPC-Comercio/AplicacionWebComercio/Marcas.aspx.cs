using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class Marcas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvMarcas.DataSource = new MarcaNegocio().Listar();
            dgvMarcas.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Marca m = new Marca { Descripcion = txtDescripcion.Text.Trim() };
            MarcaNegocio negocio = new MarcaNegocio();

            if (hfId.Value == "0")
                negocio.Agregar(m);
            else
            {
                m.Id = int.Parse(hfId.Value);
                negocio.Modificar(m);
            }

            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvMarcas.EditIndex = -1;
            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void dgvMarcas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Marca m = new MarcaNegocio().Listar().Find(x => x.Id == id);

            hfId.Value = m.Id.ToString();
            txtDescripcion.Text = m.Descripcion;

            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvMarcas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = (int)dgvMarcas.DataKeys[e.RowIndex].Value;
                new MarcaNegocio().Eliminar(id);
                lblError.Visible = false;
            }
            catch (Exception ex)
            {
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
