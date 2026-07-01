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
            var productos = new ProductoNegocio().Listar();
            ddlProducto.DataSource     = productos;
            ddlProducto.DataTextField  = "NombreProducto";
            ddlProducto.DataValueField = "Id";
            ddlProducto.DataBind();
            ddlProducto.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Seleccionar producto --", "0"));
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
            Producto producto = new ProductoNegocio().Listar().Find(p => p.Id == idProducto);
            decimal ganancia  = producto != null ? producto.PorcentajeGanancia : 0;

            var negocio = new TrazabilidadNegocio();

            var lotes = negocio.ObtenerLotes(idProducto, ganancia);
            dgvLotes.DataSource = lotes;
            dgvLotes.DataBind();
            litStockActual.Text = lotes.Sum(l => l.CantidadDisp).ToString();

            var ventas = negocio.ObtenerVentas(idProducto);
            dgvVentas.DataSource = ventas;
            dgvVentas.DataBind();

            pnlResultados.Visible = true;
        }
    }
}
