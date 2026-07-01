using System;
using System.Collections.Generic;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class FacturaReporte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    int idFactura;
                    if (int.TryParse(Request.QueryString["id"], out idFactura))
                    {
                        CargarFactura(idFactura);
                    }
                    else
                    {
                        MostrarError("ID de factura inválido.");
                    }
                }
                else
                {
                    MostrarError("No se especificó la factura a imprimir.");
                }
            }
        }

        private void CargarFactura(int idFactura)
        {
            try
            {
                FacturaNegocio facturaNegocio = new FacturaNegocio();
                DetalleFacturaNegocio detalleNegocio = new DetalleFacturaNegocio();

                // Buscar la factura por ID de la lista principal
                List<Factura> facturas = facturaNegocio.Listar();
                Factura factura = facturas.FirstOrDefault(f => f.Id == idFactura);

                if (factura != null)
                {
                    // Valida que el usuario logueado tenga permiso para ver solo su factura
                    Usuario u = Session["Usuario"] as Usuario;

                    if (Seguridad.EsCliente(u))
                    {
                        if (factura.Cliente.ID != u.IdEntidad)
                        {
                            Response.Redirect("Default.aspx", false);
                            return;
                        }
                    }

                    lblNumeroFactura.InnerText = "Factura N°: " + factura.NumeroFactura;
                    lblFecha.InnerText         = "Fecha: " + factura.Fecha.ToString("dd/MM/yyyy HH:mm");
                    lblClienteNombre.InnerText = factura.Cliente.Nombre + " " + factura.Cliente.Apellido;

                    litDatosFactura.Text = "<p>Factura N°: <strong>" + factura.NumeroFactura + "</strong></p>";
                    litDatosFactura.Text += "<p>Fecha: " + factura.Fecha.ToString("dd/MM/yyyy") + "</p>";
                    litDatosFactura.Text += "<p>Total: <strong>" + factura.Total.ToString("C") + "</strong></p>";

                    List<DetalleFactura> detalles = detalleNegocio.ListarPorFactura(idFactura);
                    dgvDetalles.DataSource = detalles;
                    dgvDetalles.DataBind();

                    lblTotalFinal.InnerText = factura.Total.ToString("C");
                }
                else
                {
                    MostrarError("No se encontró la factura solicitada.");
                }
            }
            catch (Exception ex)
            {
                MostrarError("Ocurrió un error al cargar el reporte: " + ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.Visible = true;
            dgvDetalles.Visible = false;
        }
    }
}
