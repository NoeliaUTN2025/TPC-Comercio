using System;
using System.Collections.Generic;
using System.Linq;
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
                Session["itemsCompra"] = new List<DetalleCompra>();
                try
                {
                    CargarDropdowns();
                    CargarPropuestas();
                }
                catch (Exception ex)
                {
                    MostrarError("Error al cargar los datos: " + ex.Message);
                }
            }
            ActualizarGrilla();
        }

        private void CargarPropuestas()
        {
            var pendientes = new PropuestaNegocio().ListarPendientes();
            if (pendientes.Count > 0)
            {
                pnlPropuestas.Visible = true;
                dgvPropuestas.DataSource = pendientes.Select(p => new {
                    p.Id,
                    RazonSocial    = p.Proveedor.RazonSocial,
                    NombreProducto = p.Producto.NombreProducto,
                    p.Cantidad,
                    p.PrecioUnitario,
                    p.Fecha
                }).ToList();
                dgvPropuestas.DataBind();
            }
            else
            {
                pnlPropuestas.Visible = false;
            }
        }

        protected void dgvPropuestas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Aprobar") return;

            int idPropuesta = int.Parse(e.CommandArgument.ToString());
            try
            {
                PropuestaNegocio propNegocio = new PropuestaNegocio();
                PropuestaProveedor propuesta = propNegocio.ObtenerPorId(idPropuesta);

                DetalleCompra detalle = new DetalleCompra
                {
                    Producto       = propuesta.Producto,
                    Cantidad       = propuesta.Cantidad,
                    PrecioUnitario = propuesta.PrecioUnitario,
                    Subtotal       = propuesta.Cantidad * propuesta.PrecioUnitario
                };

                Compra compra = new Compra
                {
                    Proveedor = propuesta.Proveedor,
                    Usuario   = new Usuario { Id = ((Usuario)Session["Usuario"]).Id }
                };

                int idCompra = new CompraNegocio().RegistrarCompra(compra, new List<DetalleCompra> { detalle });
                propNegocio.Aprobar(idPropuesta);
                Response.Redirect("~/CompraReporte.aspx?id=" + idCompra, false);
            }
            catch (Exception ex)
            {
                MostrarError("Error al aprobar la propuesta: " + ex.Message);
            }
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
            List<DetalleCompra> items = Session["itemsCompra"] as List<DetalleCompra>;
            if (items == null) items = new List<DetalleCompra>();

            dgvItems.DataSource = items;
            dgvItems.DataBind();

            decimal total = 0;
            foreach (DetalleCompra d in items)
                total += d.Subtotal;
            lblTotal.Text = total.ToString("C");
        }

        private void MostrarError(string msg)
        {
            lblMensaje.Text = msg;
            lblMensaje.CssClass = "alert alert-danger d-block w-100 mb-3";
            lblMensaje.Visible = true;
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
            compra.Usuario   = new Usuario   { Id = ((Usuario)Session["Usuario"]).Id };

            try
            {
                int idCompra = new CompraNegocio().RegistrarCompra(compra, items);
                Session.Remove("itemsCompra");
                Response.Redirect("~/CompraReporte.aspx?id=" + idCompra, false);
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
