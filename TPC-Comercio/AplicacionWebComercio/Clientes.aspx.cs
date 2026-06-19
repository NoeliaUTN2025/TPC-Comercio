using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class Clientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            dgvClientes.DataSource = new ClienteNegocio().Listar();
            dgvClientes.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblError.Visible = false;
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                Cliente c = new Cliente
                {
                    DNI      = txtDNI.Text.Trim(),
                    Nombre   = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                    Telefono  = string.IsNullOrWhiteSpace(txtTelefono.Text)  ? null : txtTelefono.Text.Trim(),
                    Email     = string.IsNullOrWhiteSpace(txtEmail.Text)     ? null : txtEmail.Text.Trim()
                };

                ClienteNegocio negocio = new ClienteNegocio();

                if (hfId.Value == "0")
                    negocio.Agregar(c);
                else
                {
                    c.ID = int.Parse(hfId.Value);
                    negocio.Modificar(c);
                }

                lblError.Visible = false;
                pnlFormulario.Visible = false;
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

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
            lblError.Visible = false;
            CargarGrilla();
        }

        protected void dgvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Cliente c = new ClienteNegocio().Listar().Find(x => x.ID == id);
            if (c == null) return;

            hfId.Value       = c.ID.ToString();
            txtDNI.Text      = c.DNI;
            txtNombre.Text   = c.Nombre;
            txtApellido.Text = c.Apellido;
            txtDireccion.Text = c.Direccion ?? "";
            txtTelefono.Text  = c.Telefono  ?? "";
            txtEmail.Text     = c.Email     ?? "";

            lblError.Visible = false;
            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvClientes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = (int)dgvClientes.DataKeys[e.RowIndex].Value;
                new ClienteNegocio().EliminarLogico(id);
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
            hfId.Value        = "0";
            txtDNI.Text       = "";
            txtNombre.Text    = "";
            txtApellido.Text  = "";
            txtDireccion.Text = "";
            txtTelefono.Text  = "";
            txtEmail.Text     = "";
        }
    }
}