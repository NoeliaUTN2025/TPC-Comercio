using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AplicacionWebComercio
{
    public partial class CambiarContrasena : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNueva.Text != txtConfirmar.Text)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Las contraseñas no coinciden.');", true);
                return;
            }

            Usuario usuario = (Usuario)Session["Usuario"];

            if (usuario.Contraseña != txtActual.Text)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('La contraseña actual es incorrecta.');", true);
                return;
            }

            UsuarioNegocio negocio = new UsuarioNegocio();

            negocio.CambiarContraseña(usuario.Id, txtActual.Text, txtNueva.Text);

            // actualizar la contraseña en la sesión
            usuario.Contraseña = txtNueva.Text;
            Session["Usuario"] = usuario;   

            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Contraseña cambiada exitosamente.'); window.location.href = 'default.aspx';", true);
        }
    }
}