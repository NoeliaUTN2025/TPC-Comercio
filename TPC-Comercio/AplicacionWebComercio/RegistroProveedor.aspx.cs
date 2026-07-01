using System;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class RegistroProveedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Seguridad.SesionActiva(Session["Usuario"]))
                Response.Redirect("~/Default.aspx");
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            UsuarioNegocio usuarioNegocio = new UsuarioNegocio();

            if (usuarioNegocio.ExisteUsuario(txtUsuario.Text.Trim()))
            {
                MostrarError("El nombre de usuario ya está en uso. Elegí otro.");
                return;
            }

            try
            {
                ProveedorNegocio proveedorNegocio = new ProveedorNegocio();

                Proveedor nuevo = new Proveedor();
                nuevo.RazonSocial = txtRazonSocial.Text.Trim();
                nuevo.Cuit = txtCuit.Text.Trim();
                nuevo.Direccion = txtDireccion.Text.Trim();
                nuevo.Telefono = txtTelefono.Text.Trim();
                nuevo.Email = txtEmail.Text.Trim();

                int idProveedor = proveedorNegocio.Agregar(nuevo);

                Usuario nuevoUsuario = new Usuario();
                nuevoUsuario.User = txtUsuario.Text.Trim();
                nuevoUsuario.Contraseña = txtContrasena.Text;
                nuevoUsuario.perfil = new Perfil { Id = 4 };
                nuevoUsuario.Estado = true;
                nuevoUsuario.IdEntidad = idProveedor;
                usuarioNegocio.AgregarUsuario(nuevoUsuario);

                Response.Redirect("~/Login.aspx?registro=ok");
            }
            catch (Exception ex)
            {
                MostrarError("Error al crear la cuenta: " + ex.Message);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtRazonSocial.Text)) { MostrarError("La Razón Social es obligatoria."); return false; }
            if (string.IsNullOrWhiteSpace(txtCuit.Text))        { MostrarError("El CUIT es obligatorio."); return false; }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))     { MostrarError("El nombre de usuario es obligatorio."); return false; }
            if (string.IsNullOrWhiteSpace(txtContrasena.Text))  { MostrarError("La contraseña es obligatoria."); return false; }
            if (txtContrasena.Text != txtConfirmar.Text)        { MostrarError("Las contraseñas no coinciden."); return false; }
            return true;
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = "alert alert-danger d-block w-100 mb-3";
        }
    }
}
