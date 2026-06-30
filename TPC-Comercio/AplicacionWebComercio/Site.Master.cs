using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             if (!Seguridad.SesionActiva(Session["Usuario"]))
             {
                if (!Request.Url.AbsolutePath.ToLower().Contains("Login"))
                {
                  //  Response.Redirect("Login.aspx");
                }
             }

        }


    }
}