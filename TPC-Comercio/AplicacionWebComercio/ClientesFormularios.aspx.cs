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
            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {   
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El campo DNI es obligatorio.');", true);
                return;
            }
            if (!txtDNI.Text.All(char.IsDigit))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El campo DNI debe contener solo números.');", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El campo Nombre es obligatorio.');", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El campo Apellido es obligatorio.');", true);
                return;
            }

            ClienteNegocio negocio = new ClienteNegocio();

            //Solo validar duplicados cuando es un alta de cliente, no cuando es una modificación
            if (Request.QueryString["id"] == null)
            {
                bool existeDNI = negocio.Listar().Exists(x => x.DNI == txtDNI.Text);

                if (existeDNI)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Ya existe un cliente con el mismo DNI.');", true);
                    return;
                }
            }           
                   


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

         protected void btnCancelar_Click(object sender, EventArgs e)
         {

             Response.Redirect("Clientes.aspx");

         }

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
