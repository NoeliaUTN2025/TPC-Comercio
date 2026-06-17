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
        protected void btnGuardar_Click(object sender, EventArgs e)

        {
            Cliente nuevo = new Cliente();

            nuevo.DNI = txtDNI.Text;
            nuevo.Nombre = txtNombre.Text;
            nuevo.Apellido = txtApellido.Text;
            nuevo.Direccion = txtDireccion.Text;
            nuevo.Telefono = txtTelefono.Text; 
            nuevo.Email = txtEmail.Text;
            
          

            ClienteNegocio negocio = new ClienteNegocio();
            negocio.AgregarCliente(nuevo);

            Response.Redirect("Clientes.aspx");
        }
    }
}