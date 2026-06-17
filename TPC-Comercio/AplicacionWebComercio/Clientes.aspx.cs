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
        public int  IdSeleccionado; 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClienteNegocio negocio = new ClienteNegocio();
                dgvClientes.DataSource = negocio.Listar();
                dgvClientes.DataBind();
            }
        }


        protected void dgvClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string id = dgvClientes.SelectedRow.Cells[1].Text;
            {
                Response.Redirect("ClientesFormularios.aspx?id=" + id);
            }

        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(dgvClientes.SelectedRow.Cells[1].Text);
            ClienteNegocio negocio = new ClienteNegocio();
            negocio.EliminarLogico(id);

            Response.Redirect("Clientes.aspx");
            
    
        }
    }
}