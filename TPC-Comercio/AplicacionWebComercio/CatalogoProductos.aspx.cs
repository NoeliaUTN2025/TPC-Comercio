using System;
using System.Collections.Generic;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class CatalogoProductos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u = Session["Usuario"] as Usuario;
            if (!Seguridad.EsCliente(u))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarProductos();
                Session["itemsCatalogo"] = new List<ItemCatalogo>();
            }
            ActualizarCarrito();
        }

        private void CargarProductos()
        {
            var productos = new ProductoNegocio().Listar()
                .Where(p => p.StockActual > 0)
                .ToList();

            ddlProducto.DataSource     = productos;
            ddlProducto.DataTextField  = "NombreProducto";
            ddlProducto.DataValueField = "Id";
            ddlProducto.DataBind();
            ddlProducto.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccionar producto --", "0"));

            ActualizarPrecio(productos);
        }

        private void ActualizarPrecio(List<Producto> productos)
        {
            if (ddlProducto.SelectedValue == "0")
            {
                lblPrecioUnitario.Text = "$0,00";
                return;
            }
            int id = int.Parse(ddlProducto.SelectedValue);
            Producto p = productos.Find(x => x.Id == id);
            if (p != null)
                lblPrecioUnitario.Text = (p.Precio * (1 + p.PorcentajeGanancia / 100m)).ToString("C");
        }

        private void ActualizarPrecio()
        {
            ActualizarPrecio(new ProductoNegocio().Listar());
        }

        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarPrecio();
        }

        protected void btnAgregarItem_Click(object sender, EventArgs e)
        {
            if (ddlProducto.SelectedValue == "0")
            {
                MostrarError("Seleccioná un producto.");
                return;
            }
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarError("Ingresá una cantidad válida.");
                return;
            }

            int idProducto = int.Parse(ddlProducto.SelectedValue);
            Producto prod  = new ProductoNegocio().Listar().Find(x => x.Id == idProducto);

            if (prod.StockActual < cantidad)
            {
                MostrarError("Stock insuficiente. Disponible: " + prod.StockActual + " unidades.");
                return;
            }

            decimal precioVenta = prod.Precio * (1 + prod.PorcentajeGanancia / 100m);

            var items = (List<ItemCatalogo>)Session["itemsCatalogo"];
            var existente = items.Find(i => i.IdProducto == idProducto);
            if (existente != null)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                items.Add(new ItemCatalogo
                {
                    IdProducto           = idProducto,
                    NombreProducto       = prod.NombreProducto,
                    Cantidad             = cantidad,
                    PorcentajeGanancia   = prod.PorcentajeGanancia,
                    PrecioVenta          = precioVenta
                });
            }

            MostrarExito("Producto agregado al carrito.");
            txtCantidad.Text = "1";
            ActualizarCarrito();
        }

        protected void dgvItems_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Quitar") return;
            int index = int.Parse(e.CommandArgument.ToString());
            var items = (List<ItemCatalogo>)Session["itemsCatalogo"];
            items.RemoveAt(index);
            ActualizarCarrito();
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            var items = (List<ItemCatalogo>)Session["itemsCatalogo"];
            if (items == null || items.Count == 0)
            {
                MostrarError("El carrito está vacío.");
                return;
            }

            Usuario u = Session["Usuario"] as Usuario;

            Factura factura = new Factura
            {
                Cliente = new Cliente { ID = u.IdEntidad },
                Usuario = new Usuario { Id = u.Id }
            };

            List<DetalleFactura> detalles = new List<DetalleFactura>();
            foreach (var item in items)
            {
                detalles.Add(new DetalleFactura
                {
                    Producto           = new Producto { Id = item.IdProducto, NombreProducto = item.NombreProducto },
                    Cantidad           = item.Cantidad,
                    PorcentajeGanancia = item.PorcentajeGanancia
                });
            }

            try
            {
                int idFactura = new FacturaNegocio().RegistrarVenta(factura, detalles);
                Session.Remove("itemsCatalogo");
                Response.Redirect("FacturaReporte.aspx?id=" + idFactura, false);
            }
            catch (Exception ex)
            {
                MostrarError("Error al procesar la compra: " + ex.Message);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("itemsCatalogo");
            Response.Redirect("CatalogoProductos.aspx");
        }

        private void ActualizarCarrito()
        {
            var items = Session["itemsCatalogo"] as List<ItemCatalogo>;
            if (items == null) { items = new List<ItemCatalogo>(); Session["itemsCatalogo"] = items; }

            dgvItems.DataSource = items.Select(i => new
            {
                i.NombreProducto,
                i.Cantidad,
                i.PrecioVenta,
                Subtotal = i.Cantidad * i.PrecioVenta
            }).ToList();
            dgvItems.DataBind();

            decimal total = items.Sum(i => i.Cantidad * i.PrecioVenta);
            litTotal.Text = total.ToString("C");
        }

        private void MostrarError(string msg)
        {
            lblMensaje.Text = msg;
            lblMensaje.CssClass = "alert alert-danger d-block w-100 mb-3";
        }

        private void MostrarExito(string msg)
        {
            lblMensaje.Text = msg;
            lblMensaje.CssClass = "alert alert-success d-block w-100 mb-3";
        }
    }

    public class ItemCatalogo
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}
