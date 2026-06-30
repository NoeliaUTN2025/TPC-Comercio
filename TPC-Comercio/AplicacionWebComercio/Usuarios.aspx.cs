using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using Negocio;


namespace AplicacionWebComercio
{
    public partial class Usuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El campo Usuario es obligatorio.');", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El campo Contraseña es obligatorio.');", true);
                return;
            }
            if (txtContraseña.Text != txtConfirmarContraseña.Text)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Las contraseñas no coinciden.');", true);
                return;
            }
            
            Usuario nuevo = new Usuario();

            nuevo.User = txtUsuario.Text;
            nuevo.Contraseña = txtContraseña.Text;
           // nuevo.Contraseña = txtConfirmarContraseña.Text;
            nuevo.Estado = chkEstado.Checked;
            nuevo.perfil = new Perfil();
            nuevo.perfil.Id = int.Parse(ddlIdPerfil.SelectedValue);

            UsuarioNegocio negocio = new UsuarioNegocio();

            if (negocio.ExisteUsuario(txtUsuario.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El usuario ya existe.');", true);
                return;
            }

           
            negocio.AgregarUsuario(nuevo);
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Usuario agregado correctamente.');", true);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {

        }
    }
}
