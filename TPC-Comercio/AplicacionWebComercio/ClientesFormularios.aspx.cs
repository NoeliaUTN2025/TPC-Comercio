using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;


namespace AplicacionWebComercio
{
    public partial class ClientesFormularios : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["ID"] != null)
                {
                    btnEliminar.Visible = true;
                    chkConfirmarEliminacion.Visible = true;

                    int id = int.Parse(Request.QueryString["id"]);
                    ClienteNegocio negocio = new ClienteNegocio();
                    Cliente seleccionado = negocio.Listar().Find(x => x.ID == id);


                    txtDNI.Text = seleccionado.DNI;
                    txtNombre.Text = seleccionado.Nombre;
                    txtApellido.Text = seleccionado.Apellido;
                    txtDireccion.Text = seleccionado.Direccion;
                    txtTelefono.Text = seleccionado.Telefono;
                    txtEmail.Text = seleccionado.Email;

                    btnGuardar.Text = "Guardar";
                }
                else
                {
                    btnEliminar.Visible = false;
                    chkConfirmarEliminacion.Visible = false;
                }


            }
        }




        protected void btnGuardar_Click(object sender, EventArgs e)

        {
            Cliente nuevo = new Cliente();

            if (Request.QueryString["id"] != null)
            {
                nuevo.ID = int.Parse(Request.QueryString["id"]);
            }

            nuevo.DNI = txtDNI.Text;
            nuevo.Nombre = txtNombre.Text;
            nuevo.Apellido = txtApellido.Text;
            nuevo.Direccion = txtDireccion.Text;
            nuevo.Telefono = txtTelefono.Text;
            nuevo.Email = txtEmail.Text;



            ClienteNegocio negocio = new ClienteNegocio();
            if (Request.QueryString["id"] != null)
            {
                negocio.Modificar(nuevo);
            }
            else
            {
                negocio.AgregarCliente(nuevo);
            }

            Response.Redirect("Clientes.aspx");
        }

        /* protected void btnCancelar_Click(object sender, EventArgs e)
         {

             Response.Redirect("Clientes.aspx");

         }*/

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            if (chkConfirmarEliminacion.Checked)
            {

                int id = int.Parse(Request.QueryString["id"]);
                ClienteNegocio negocio = new ClienteNegocio();
                negocio.EliminarLogico(id);

                Response.Redirect("Clientes.aspx");
            }
        }


    }
}
