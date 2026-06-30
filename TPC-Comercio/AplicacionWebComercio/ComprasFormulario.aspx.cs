using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class ComprasFormulario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDropdowns();
                Session["itemsCompra"] = new List<DetalleCompra>();
            }
            ActualizarGrilla();
        }

        private void CargarDropdowns()
        {
            ddlProveedor.DataSource     = new ProveedorNegocio().Listar();
            ddlProveedor.DataTextField  = "RazonSocial";
            ddlProveedor.DataValueField = "ID";
            ddlProveedor.DataBind();

            ddlProducto.DataSource     = new ProductoNegocio().Listar();
            ddlProducto.DataTextField  = "NombreProducto";
            ddlProducto.DataValueField = "Id";
            ddlProducto.DataBind();
        }

        private void ActualizarGrilla()
        {
            List<DetalleCompra> items = (List<DetalleCompra>)Session["itemsCompra"];
            dgvItems.DataSource = items;
            dgvItems.DataBind();

            decimal total = 0;
            foreach (DetalleCompra d in items)
                total += d.Subtotal;
            lblTotal.Text = total.ToString("C");
        }

        protected void btnAgregarItem_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                lblError.Text = "Ingrese una cantidad válida.";
                lblError.Visible = true;
                return;
            }

            if (!decimal.TryParse(txtPrecioUnitario.Text, out decimal precio) || precio <= 0)
            {
                lblError.Text = "Ingrese un precio unitario válido.";
                lblError.Visible = true;
                return;
            }

            lblError.Visible = false;

            int idProducto = int.Parse(ddlProducto.SelectedValue);
            Producto prod = new ProductoNegocio().Listar().Find(x => x.Id == idProducto);

            DetalleCompra item = new DetalleCompra();
            item.Producto       = prod;
            item.Cantidad       = cantidad;
            item.PrecioUnitario = precio;
            item.Subtotal       = cantidad * precio;

            List<DetalleCompra> items = (List<DetalleCompra>)Session["itemsCompra"];
            items.Add(item);
            ActualizarGrilla();
        }

        protected void dgvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Quitar") return;

            int index = int.Parse(e.CommandArgument.ToString());
            List<DetalleCompra> items = (List<DetalleCompra>)Session["itemsCompra"];
            items.RemoveAt(index);
            ActualizarGrilla();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            List<DetalleCompra> items = (List<DetalleCompra>)Session["itemsCompra"];

            if (items.Count == 0)
            {
                lblError.Text = "Debe agregar al menos un producto.";
                lblError.Visible = true;
                return;
            }

            Compra compra = new Compra();
            compra.Proveedor = new Proveedor { ID = int.Parse(ddlProveedor.SelectedValue) };
            compra.Usuario   = new Usuario   { Id = 1 };

            try
            {
                new CompraNegocio().RegistrarCompra(compra, items);
                Session.Remove("itemsCompra");
                Response.Redirect("Compras.aspx", false);
            }
            catch (Exception ex)
            {
                lblError.Text = "Error al registrar la compra: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("itemsCompra");
            Response.Redirect("Compras.aspx");
        }
    }
}
