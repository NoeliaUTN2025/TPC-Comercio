using Negocio;
using System;

namespace AplicacionWebComercio
{
    public partial class Marcas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MarcaNegocio negocio = new MarcaNegocio();
                dgvMarcas.DataSource = negocio.Listar();
                dgvMarcas.DataBind();
            }
        }
    }
}