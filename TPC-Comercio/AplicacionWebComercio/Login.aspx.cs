using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(Object sender, EventArgs e)
        {
            UsuarioNegocio negocio = new UsuarioNegocio();
            Usuario usuario;

            try
            {
                usuario = negocio.Login(txtUsuario.Text, txtPassword.Text);

                if (usuario != null)
                {
                    Session["Usuario"]= usuario;
                    Response.Redirect("Default.aspx");
                        
                }
                else
                {
                    lblError.Text = "Usuario o contraseña incorrectos."; 
                }

            }
            catch (Exception ex)
            {

                lblError.Text = ex.Message;
            }
        }
    }
}