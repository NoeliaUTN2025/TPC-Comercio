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
                if (Seguridad.SesionActiva(u) && (Seguridad.EsAdmin(u) || Seguridad.EsVendedor(u)))
                {
                    pnlDashboard.Visible = true;
                    pnlMensajeAnonimo.Visible = false;

                    try
                    {
                        DashboardNegocio negocio = new DashboardNegocio();
                        DashboardStats stats = negocio.ObtenerEstadisticas();

                        litTotalVentas.Text = "$" + stats.TotalVentasMes.ToString("0.00");
                        litTotalCompras.Text = "$" + stats.TotalComprasMes.ToString("0.00");
                        litBajoStock.Text = stats.ProductosBajoStock.ToString();
                    }
                    catch (Exception ex)
                    {
                        litTotalVentas.Text = "Error";
                        litTotalCompras.Text = "Error";
                        litBajoStock.Text = "Error";
                    }
                }
                else
                {
                    pnlDashboard.Visible = false;
                    pnlMensajeAnonimo.Visible = true;
                }
            }
        }
    }
}