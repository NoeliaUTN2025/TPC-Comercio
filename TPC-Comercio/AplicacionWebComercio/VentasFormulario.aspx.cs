using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class VentasFormulario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDropdowns();
                Session["itemsVenta"] = new List<DetalleFactura>();
            }
            ActualizarGrilla();
        }

        private void CargarDropdowns()
        {
            ddlCliente.DataSource = new ClienteNegocio().Listar();
            ddlCliente.DataTextField = "Nombre"; 
            ddlCliente.DataValueField = "ID";
            ddlCliente.DataBind();

            ddlProducto.DataSource = new ProductoNegocio().Listar();
            ddlProducto.DataTextField = "NombreProducto";
            ddlProducto.DataValueField = "Id";
            ddlProducto.DataBind();
        }

        private void ActualizarGrilla()
        {
            List<DetalleFactura> items = (List<DetalleFactura>)Session["itemsVenta"];
            dgvItems.DataSource = items;
            dgvItems.DataBind();
        }

        protected void btnAgregarItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("Ingrese una cantidad válida.", false);
                return;
            }

            int idProducto = int.Parse(ddlProducto.SelectedValue);
            Producto prod = new ProductoNegocio().Listar().Find(x => x.Id == idProducto);

            // Validacion del stock global antes de meter al carrito
            if (prod.StockActual < cantidad)
            {
                MostrarMensaje("Stock insuficiente. Stock actual de " + prod.NombreProducto + ": " + prod.StockActual, false);
                return;
            }

            DetalleFactura item = new DetalleFactura();
            item.Producto = prod;
            item.Cantidad = cantidad;
            item.PorcentajeGanancia = prod.PorcentajeGanancia; 

            List<DetalleFactura> items = (List<DetalleFactura>)Session["itemsVenta"];
            items.Add(item);
            
            MostrarMensaje("", true);
            txtCantidad.Text = "";
            ActualizarGrilla();
        }

        protected void dgvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Quitar") return;

            int index = int.Parse(e.CommandArgument.ToString());
            List<DetalleFactura> items = (List<DetalleFactura>)Session["itemsVenta"];
            items.RemoveAt(index);
            ActualizarGrilla();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            List<DetalleFactura> items = (List<DetalleFactura>)Session["itemsVenta"];

            if (items.Count == 0)
            {
                MostrarMensaje("Debe agregar al menos un producto al carrito.", false);
                return;
            }

            Factura factura = new Factura();
            factura.Cliente = new Cliente { ID = int.Parse(ddlCliente.SelectedValue) };
            factura.Usuario = new Usuario { Id = 1 }; 
            try
            {
                int idGenerado = new FacturaNegocio().RegistrarVenta(factura, items);
                Session.Remove("itemsVenta");
                Response.Redirect("FacturaReporte.aspx?id=" + idGenerado, false);
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, false);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("itemsVenta");
            Response.Redirect("Ventas.aspx", false);
        }

        private void MostrarMensaje(string mensaje, bool exito)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = exito ? "mt-2 d-block fw-bold text-success" : "mt-2 d-block fw-bold text-danger";
        }
    }
}
