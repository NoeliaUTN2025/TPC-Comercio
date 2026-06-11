using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Negocio;


namespace AplicacionWebComercio
{
    public partial class Clientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClienteNegocio negocio = new ClienteNegocio();
                dgvClientes.DataSource = negocio.Listar();
                dgvClientes.DataBind();
            }
        }
    }
}