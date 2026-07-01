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
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Usuario u = Session["Usuario"] as Usuario;
                if (Seguridad.SesionActiva(u))
                {
                    if (Seguridad.EsAdmin(u) || Seguridad.EsVendedor(u))
                    {
                        pnlDashboard.Visible = true;
                        pnlActividadReciente.Visible = false;
                        pnlMensajeAnonimo.Visible = false;

                        try
                        {
                            DashboardNegocio negocio = new DashboardNegocio();
                            DashboardStats stats = negocio.ObtenerEstadisticas();

                            litTotalVentas.Text = "$" + stats.TotalVentasMes.ToString("0.00");
                            litTotalCompras.Text = "$" + stats.TotalComprasMes.ToString("0.00");
                            litBajoStock.Text = stats.ProductosBajoStock.ToString();
                        }
                        catch (Exception)
                        {
                            litTotalVentas.Text = "Error";
                            litTotalCompras.Text = "Error";
                            litBajoStock.Text = "Error";
                        }
                    }
                    else // Es Cliente o Proveedor
                    {
                        pnlDashboard.Visible = false;
                        pnlActividadReciente.Visible = true;
                        pnlMensajeAnonimo.Visible = false;

                        try
                        {
                            DashboardNegocio negocio = new DashboardNegocio();
                            if (Seguridad.EsCliente(u))
                            {
                                var compras = negocio.ObtenerUltimasComprasCliente(u.IdEntidad);
                                if (compras != null && compras.Count > 0)
                                {
                                    rptActividad.DataSource = compras.Select(c => new {
                                        NumeroFactura = c.NumeroFactura,
                                        Fecha = c.Fecha,
                                        Total = c.Total,
                                        estado = c.estado
                                    }).ToList();
                                    rptActividad.DataBind();
                                }
                                else
                                {
                                    lblSinActividad.Visible = true;
                                }
                            }
                            else // Proveedor
                            {
                                var ventas = negocio.ObtenerUltimasPropuestasProveedor(u.IdEntidad);
                                if (ventas != null && ventas.Count > 0)
                                {
                                    rptActividad.DataSource = ventas.Select(v => new {
                                        NumeroFactura = "L" + v.Id.ToString("D5"),
                                        Fecha = v.Fecha,
                                        Total = v.Total,
                                        estado = v.estado
                                    }).ToList();
                                    rptActividad.DataBind();
                                }
                                else
                                {
                                    lblSinActividad.Visible = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            lblSinActividad.Text = "Hubo un error al cargar su actividad.";
                            lblSinActividad.Visible = true;
                        }
                    }
                }
                else
                {
                    pnlDashboard.Visible = false;
                    pnlActividadReciente.Visible = false;
                    pnlMensajeAnonimo.Visible = true;
                }
            }
        }
    }
}