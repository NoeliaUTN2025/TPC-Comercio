using System;
using System.Linq;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class Caja : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u = Session["Usuario"] as Usuario;
            if (!Seguridad.SesionActiva(u) || (!Seguridad.EsAdmin(u) && !Seguridad.EsVendedor(u)))
            {
                Response.Redirect("Default.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                CargarEstado();
            }
        }

        private void CargarEstado()
        {
            Dominio.Caja cajaAbierta = new CajaNegocio().ObtenerCajaAbierta();

            pnlAbrirCaja.Visible = (cajaAbierta == null);
            pnlCajaAbierta.Visible = (cajaAbierta != null);

            if (cajaAbierta != null)
            {
                litFechaApertura.Text = cajaAbierta.FechaApertura.ToString("dd/MM/yyyy HH:mm");
                litMontoApertura.Text = cajaAbierta.MontoApertura.ToString("C");
                litEfectivoAcumulado.Text = new PagoNegocio().SumarEfectivoDesde(cajaAbierta.FechaApertura).ToString("C");
            }

            dgvHistorico.DataSource = new CajaNegocio().Listar();
            dgvHistorico.DataBind();
        }

        protected void btnAbrirCaja_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMontoApertura.Text, out decimal monto) || monto < 0)
            {
                MostrarMensaje("Ingrese un monto inicial válido.", false);
                return;
            }

            try
            {
                Usuario u = (Usuario)Session["Usuario"];
                new CajaNegocio().AbrirCaja(monto, u.Id);
                txtMontoApertura.Text = "";
                MostrarMensaje("Caja abierta correctamente.", true);
                CargarEstado();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, false);
            }
        }

        protected void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMontoCierre.Text, out decimal monto) || monto < 0)
            {
                MostrarMensaje("Ingrese un monto de cierre válido.", false);
                return;
            }

            try
            {
                Dominio.Caja cajaAbierta = new CajaNegocio().ObtenerCajaAbierta();
                if (cajaAbierta == null)
                {
                    MostrarMensaje("No hay una caja abierta para cerrar.", false);
                    return;
                }

                Usuario u = (Usuario)Session["Usuario"];
                new CajaNegocio().CerrarCaja(cajaAbierta.Id, monto, u.Id);

                Dominio.Caja cajaCerrada = new CajaNegocio().Listar().FirstOrDefault(c => c.Id == cajaAbierta.Id);
                decimal diferencia = (cajaCerrada != null && cajaCerrada.Diferencia.HasValue) ? cajaCerrada.Diferencia.Value : 0m;

                txtMontoCierre.Text = "";
                MostrarMensaje("Caja cerrada. Diferencia de arqueo: " + diferencia.ToString("C"), true);
                CargarEstado();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, false);
            }
        }

        private void MostrarMensaje(string mensaje, bool exito)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = exito ? "alert alert-success w-100 mb-3" : "alert alert-danger w-100 mb-3";
        }
    }
}
