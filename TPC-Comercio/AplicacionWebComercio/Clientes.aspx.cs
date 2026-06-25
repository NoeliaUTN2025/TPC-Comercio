using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;
using System.Web.ModelBinding;


namespace AplicacionWebComercio
{
    public partial class Clientes : System.Web.UI.Page
    {
        public int IdSeleccionado;

        public bool ConfirmarEliminacion { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfirmarEliminacion = false;
            {
                if (!IsPostBack)
                {
                    ClienteNegocio negocio = new ClienteNegocio();

                    List<Cliente> listaClientes = negocio.Listar();


                    Session["listaClientes"] = listaClientes;
                    dgvClientes.DataSource = listaClientes.FindAll(x => x.Estado == true);
                   
                    dgvClientes.DataBind();

                }
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
            if (ConfirmarEliminacion)
            {

                int id = int.Parse(Request.QueryString["id"]);
                ClienteNegocio negocio = new ClienteNegocio();
                negocio.EliminarLogico(id);


                Response.Redirect("Clientes.aspx");

            }



        }

        protected void filtro_TextChanged(object sender, EventArgs e)
        {
            List<Cliente> Listar = (List<Cliente>)Session["listaClientes"];
            List<Cliente> listafiltrada = Listar.FindAll(x => x.Nombre.ToUpper().Contains(filtro.Text.ToUpper()) || x.Apellido.ToUpper().Contains(filtro.Text.ToUpper()) || x.DNI.ToString().Contains(filtro.Text));
            dgvClientes.DataSource = listafiltrada;
            dgvClientes.DataBind();
        }

        protected void dgvClientes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btn = (LinkButton)e.Row.Cells[0].Controls[0];
                btn.Text = "Editar";
            }
        }
        protected void chkAvanzado_CheckedChanged(object sender, EventArgs e)
        {
            filtro.Visible = !chkAvanzado.Checked;

            if (chkAvanzado.Checked)
            {
                ddlCampo.SelectedIndex = 0;
                ddlCriterio.Items.Clear();
            }

        }

        protected void ddlCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlCriterio.Items.Clear();

            if (ddlCampo.SelectedValue == "")
                return;
                
            ddlCriterio.Items.Add(new ListItem("Seleccione un criterio", ""));

            if (ddlCampo.SelectedItem.Text == "Nombre" || ddlCampo.SelectedItem.Text == "Apellido")
            {
                ddlCriterio.Items.Add("Comienza con");
                ddlCriterio.Items.Add("Contiene");
                ddlCriterio.Items.Add("Termina con");
            }
            else if (ddlCampo.SelectedItem.Text == "DNI")
            {
                ddlCriterio.Items.Add("Igual a");
                ddlCriterio.Items.Add("Mayor que");
                ddlCriterio.Items.Add("Menor que");
            }
        }


        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            List<Cliente> Lista = (List<Cliente>)Session["listaClientes"];
            List<Cliente> listafiltrada = new List<Cliente>();
            if (chkAvanzado.Checked && (ddlCampo.SelectedValue != "" && ddlCriterio.SelectedValue != ""))
            {
                // Filtros Nombre, apellido y DNI
                switch (ddlCampo.SelectedItem.Text)
                {
                    case "Nombre":

                        switch (ddlCriterio.SelectedItem.Text)
                        {
                            case "Comienza con":
                                listafiltrada = Lista.FindAll(x => x.Nombre.ToUpper().StartsWith(txtFiltroAvanzado.Text.ToUpper()));
                                break;
                            case "Contiene":
                                listafiltrada = Lista.FindAll(x => x.Nombre.ToUpper().Contains(txtFiltroAvanzado.Text.ToUpper()));
                                break;
                            case "Termina con":
                                listafiltrada = Lista.FindAll(x => x.Nombre.ToUpper().EndsWith(txtFiltroAvanzado.Text.ToUpper()));
                                break;
                        }


                        break;
                    case "Apellido":
                        switch (ddlCriterio.SelectedItem.Text)
                        {
                            case "Comienza con":
                                listafiltrada = Lista.FindAll(x => x.Apellido.ToUpper().StartsWith(txtFiltroAvanzado.Text.ToUpper()));
                                break;
                            case "Contiene":
                                listafiltrada = Lista.FindAll(x => x.Apellido.ToUpper().Contains(txtFiltroAvanzado.Text.ToUpper()));
                                break;
                            case "Termina con":
                                listafiltrada = Lista.FindAll(x => x.Apellido.ToUpper().EndsWith(txtFiltroAvanzado.Text.ToUpper()));
                                break;
                        }

                        break;
                    case "DNI":

                        int dniFiltro = int.Parse(txtFiltroAvanzado.Text);
                        switch (ddlCriterio.SelectedItem.Text)
                        {
                            case "Igual a":
                                listafiltrada = Lista.FindAll(x => int.Parse(x.DNI) == dniFiltro);
                                break;
                            case "Mayor que":
                                listafiltrada = Lista.FindAll(x => int.Parse(x.DNI) > dniFiltro);
                                break;
                            case "Menor que":
                                listafiltrada = Lista.FindAll(x => int.Parse(x.DNI) < dniFiltro);
                                break;
                        }
                        break;
                }
            }
            else
            {
                listafiltrada = Lista.FindAll(x => x.Nombre.ToUpper().Contains(filtro.Text.ToUpper()) || x.Apellido.ToUpper().Contains(filtro.Text.ToUpper()) || x.DNI.ToString().Contains(filtro.Text));
            }

            // Filtro por estado
            if (ddlEstado.SelectedValue == "Activo")
            {
                listafiltrada = listafiltrada.FindAll(x => x.Estado == true);
            }
            else if (ddlEstado.SelectedValue == "Inactivo")
            {
                listafiltrada = listafiltrada.FindAll(x => x.Estado == false);
            }
            dgvClientes.DataSource = listafiltrada;
            dgvClientes.DataBind();
        }
    }
}