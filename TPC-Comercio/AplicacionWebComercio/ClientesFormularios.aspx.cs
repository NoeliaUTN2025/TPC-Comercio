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
                if (Request.QueryString["id"] != null)
                {
                    int id = int.Parse(Request.QueryString["id"]);
                    ClienteNegocio negocio = new ClienteNegocio();
                    Cliente seleccionado = negocio.Listar().Find(x => x.ID == id);

                    {
                        txtDNI.Text = seleccionado.DNI;
                        txtNombre.Text = seleccionado.Nombre;
                        txtApellido.Text = seleccionado.Apellido;
                        txtDireccion.Text = seleccionado.Direccion;
                        txtTelefono.Text = seleccionado.Telefono;
                        txtEmail.Text = seleccionado.Email;

                        btnGuardar.Text = "Modificar";
                    }
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
    }
}