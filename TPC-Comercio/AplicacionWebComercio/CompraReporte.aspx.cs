using System;
using System.Collections.Generic;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class CompraReporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u = Session["Usuario"] as Usuario;
            if (!Seguridad.EsAdmin(u) && !Seguridad.EsVendedor(u))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int id;
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out id))
                    CargarCompra(id);
                else
                    MostrarError("No se especificó la compra a imprimir.");
            }
        }

        private void CargarCompra(int idCompra)
        {
            try
            {
                Compra compra = new CompraNegocio().ObtenerPorId(idCompra);
                if (compra == null) { MostrarError("No se encontró la compra solicitada."); return; }

                lblNumeroCompra.InnerText = "Orden de Compra N° " + compra.Id.ToString("D6");
                lblFecha.InnerText        = "Fecha: " + compra.Fecha.ToString("dd/MM/yyyy HH:mm");

                Proveedor p = compra.Proveedor;
                litProveedor.Text = "<p><strong>" + p.RazonSocial + "</strong></p>";
                litProveedor.Text += "<p>CUIT: " + p.Cuit + "</p>";
                if (!string.IsNullOrEmpty(p.Direccion))
                    litProveedor.Text += "<p>Direccion: " + p.Direccion + "</p>";

                litDatosCompra.Text = "<p>Compra N°: <strong>" + idCompra + "</strong></p>";
                litDatosCompra.Text += "<p>Fecha: " + compra.Fecha.ToString("dd/MM/yyyy") + "</p>";
                litDatosCompra.Text += "<p>Total: <strong>" + compra.Total.ToString("C") + "</strong></p>";

                List<DetalleCompra> detalles = new DetalleCompraNegocio().ListarPorCompra(idCompra);
                List<object> filas = new List<object>();
                foreach (DetalleCompra d in detalles)
                {
                    filas.Add(new {
                        NombreProducto = d.Producto.NombreProducto,
                        d.Cantidad,
                        d.PrecioUnitario,
                        d.Subtotal
                    });
                }
                dgvDetalles.DataSource = filas;
                dgvDetalles.DataBind();

                lblTotal.InnerText = compra.Total.ToString("C");
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar el reporte: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text    = mensaje;
            lblError.Visible = true;
            dgvDetalles.Visible = false;
        }
    }
}
