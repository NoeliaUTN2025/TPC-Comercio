using Negocio;
using System;

namespace AplicacionWebComercio
{
    public partial class Categorias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CategoriaNegocio negocio = new CategoriaNegocio();
                dgvCategorias.DataSource = negocio.Listar();
                dgvCategorias.DataBind();
            }
        }
    }
}