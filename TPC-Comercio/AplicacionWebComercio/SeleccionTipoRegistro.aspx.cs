using System;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class SeleccionTipoRegistro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Seguridad.SesionActiva(Session["Usuario"]))
                Response.Redirect("~/Default.aspx");
        }
    }
}
