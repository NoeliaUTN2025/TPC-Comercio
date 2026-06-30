using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
            if (!Seguridad.SesionActiva((Session)["Usuario"]))
            {
                Response.Redirect("Login.aspx", false);
                return;
            }

            if (!Seguridad.EsAdmin((Usuario)Session["Usuario"]))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No tiene permisos para acceder a esta sección.'); window.location='Default.aspx';", true);
                return;
            }
            /*if (!IsPostBack)
            {
                // Aquí puedes cargar los datos iniciales si es necesario
            }*/
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

            Response.Redirect("Login.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {

        }
    }
}
