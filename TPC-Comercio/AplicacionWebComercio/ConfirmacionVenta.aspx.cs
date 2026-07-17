using System;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class ConfirmacionVenta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int idFactura;
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out idFactura))
                {
                    CargarConfirmacion(idFactura);
                }
                else
                {
                    MostrarError("ID de factura inválido.");
                }
            }
        }

        private void CargarConfirmacion(int idFactura)
        {
            try
            {
                FacturaNegocio facturaNegocio = new FacturaNegocio();
                Factura factura = facturaNegocio.Listar().FirstOrDefault(f => f.Id == idFactura);

                if (factura == null)
                {
                    MostrarError("No se encontró la factura solicitada.");
                    return;
                }

                Usuario u = Session["Usuario"] as Usuario;
                if (Seguridad.EsCliente(u) && factura.Cliente.ID != u.IdEntidad)
                {
                    Response.Redirect("Default.aspx", false);
                    return;
                }

                litNumeroFactura.Text = factura.NumeroFactura;
                litFecha.Text = factura.Fecha.ToString("dd/MM/yyyy HH:mm");
                litCliente.Text = factura.Cliente.Nombre + " " + factura.Cliente.Apellido;
                litTotal.Text = factura.Total.ToString("C");

                dgvItems.DataSource = new DetalleFacturaNegocio().ListarPorFactura(idFactura);
                dgvItems.DataBind();

                lnkVerFactura.HRef = "FacturaReporte.aspx?id=" + idFactura;

                Pago pago = new PagoNegocio().ObtenerPorFactura(idFactura);
                if (pago != null)
                {
                    litFormaPago.Text = pago.Tipo;

                    if (pago.Tipo == "Credito")
                    {
                        pnlCuotas.Visible = true;
                        dgvCuotas.DataSource = new PagoNegocio().ListarCuotasPorPago(pago.Id);
                        dgvCuotas.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError("Ocurrió un error al cargar la confirmación: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = "alert alert-danger w-100 mb-3";
            pnlResumen.Visible = false;
        }
    }
}
