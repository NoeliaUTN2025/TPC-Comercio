using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class Proveedores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Seguridad.SesionActiva((Session)["Usuario"]))
            {
                Response.Redirect("Login.aspx", false);
                return;
            }

            Usuario usuario = (Usuario)Session["Usuario"];
            if (!(Seguridad.EsAdmin(usuario) || Seguridad.EsVendedor(usuario) || Seguridad.EsProveedor(usuario)))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No tiene permisos para acceder a esta sección.'); window.location='Default.aspx';", true);
                return;
            }

            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            Dominio.FiltrosBusqueda filtros = ctrlFiltros.ObtenerFiltros();
            var lista = new ProveedorNegocio().Listar(filtros);

            dgvProveedores.DataSource = lista;
            dgvProveedores.DataBind();
        }

        protected void ctrlFiltros_Filtrar(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Proveedor p = new Proveedor
            {
                RazonSocial = txtRazonSocial.Text.Trim(),
                Cuit = txtCuit.Text.Trim(),
                Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim()
            };

            ProveedorNegocio negocio = new ProveedorNegocio();

            if (hfId.Value == "0")
                negocio.Agregar(p);
            else
            {
                p.ID = int.Parse(hfId.Value);
                negocio.Modificar(p);
            }

            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvProveedores.EditIndex = -1;
            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void dgvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Proveedor p = new ProveedorNegocio().Listar().Find(x => x.ID == id);

            hfId.Value = p.ID.ToString();
            txtRazonSocial.Text = p.RazonSocial;
            txtCuit.Text = p.Cuit;
            txtDireccion.Text = p.Direccion;
            txtTelefono.Text = p.Telefono;
            txtEmail.Text = p.Email;

            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvProveedores_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = (int)dgvProveedores.DataKeys[e.RowIndex].Value;
            new ProveedorNegocio().EliminarLogico(id);
            CargarGrilla();
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "0";
            txtRazonSocial.Text = "";
            txtCuit.Text = "";
            txtDireccion.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
        }
    }
}
