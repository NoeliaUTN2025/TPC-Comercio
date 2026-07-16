using System;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class TrazabilidadLote : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u = Session["Usuario"] as Usuario;
            if (!Seguridad.EsAdmin(u))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
                CargarProductos();
        }

        private void CargarProductos()
        {
            try
            {
                var productos = new ProductoNegocio().Listar();
                ddlProducto.DataSource     = productos;
                ddlProducto.DataTextField  = "NombreProducto";
                ddlProducto.DataValueField = "Id";
                ddlProducto.DataBind();
                ddlProducto.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccionar producto --", "0"));
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar los productos: " + System.Web.HttpUtility.HtmlEncode(ex.Message);
                lblMensaje.Visible = true;
            }
        }

        protected void ddlProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProducto.SelectedValue == "0")
            {
                pnlResultados.Visible = false;
                return;
            }
            CargarTrazabilidad(int.Parse(ddlProducto.SelectedValue));
        }

        private void CargarTrazabilidad(int idProducto)
        {
            try
            {
                Producto producto = new ProductoNegocio().Listar().Find(p => p.Id == idProducto);
                decimal ganancia = producto != null ? producto.PorcentajeGanancia : 0;

                if (producto != null)
                { 
                    litCodigo.Text      = producto.Codigo;
                    litProducto.Text   = producto.NombreProducto;
                    litDescripcion.Text = producto.Descripcion;
                    litMarca.Text      = producto.marca.Descripcion;
                    litCategoria.Text  = producto.categoria.Descripcion;
                    litGanancia.Text   = ganancia.ToString("N2") +"%";  

                    if (!string.IsNullOrEmpty(producto.UrlImagen)) {
                        imgProducto.ImageUrl = producto.UrlImagen;
                    }
                    else
                    {
                        imgProducto.ImageUrl = "~/Images/sin-image.png";
                    }
                }
               

                TrazabilidadNegocio negocio = new TrazabilidadNegocio();

                var lotes = negocio.ObtenerLotes(idProducto, ganancia);
                dgvLotes.DataSource = lotes;
                dgvLotes.DataBind();
                litStockActual.Text = lotes.Sum(l => l.CantidadDisp).ToString();

                var ventas = negocio.ObtenerVentas(idProducto);
                dgvVentas.DataSource = ventas;
                dgvVentas.DataBind();

                pnlResultados.Visible = true;
            }
            catch (Exception ex)
            {
                pnlResultados.Visible = false;
                lblMensaje.Text = "Error al cargar la trazabilidad: " + System.Web.HttpUtility.HtmlEncode(ex.Message);
                lblMensaje.Visible = true;
            }
        }
    }
}
