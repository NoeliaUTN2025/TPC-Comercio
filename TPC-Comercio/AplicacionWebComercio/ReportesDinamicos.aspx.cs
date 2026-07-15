using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class ReportesDinamicos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dominio.Usuario usuario = (Dominio.Usuario)Session["Usuario"];
                if (!Negocio.Seguridad.EsAdmin(usuario))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Solo los administradores pueden acceder a Reportes.'); window.location='Default.aspx';", true);
                    return;
                }
            }
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void CargarReporte()
        {
            lblMensaje.Text = "";
            try
            {
                if (string.IsNullOrEmpty(ddlTipoReporte.SelectedValue))
                {
                    lblMensaje.Text = "Por favor, seleccione un tipo de reporte.";
                    return;
                }

                DateTime? desde = null;
                DateTime? hasta = null;

                if (DateTime.TryParse(txtFechaDesde.Text, out DateTime fd)) desde = fd;
                if (DateTime.TryParse(txtFechaHasta.Text, out DateTime fh)) hasta = fh;

                if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
                {
                    lblMensaje.Text = "La fecha 'Desde' no puede ser mayor que 'Hasta'.";
                    return;
                }

                ReportesNegocio negocio = new ReportesNegocio();
                DataTable dt = negocio.GenerarReporte(ddlTipoReporte.SelectedValue, desde, hasta);

                dgvReporte.DataSource = dt;
                dgvReporte.DataBind();

                // Guardar en ViewState o Session para exportar después (o regenerarlo en el click de exportar)
                Session["UltimoReporte"] = dt;
                btnExportar.Visible = dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al generar reporte: " + ex.Message;
            }
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            if (Session["UltimoReporte"] is DataTable dt)
            {
                string csv = GenerarCsv(dt);
                
                string filename = "Reporte_" + ddlTipoReporte.SelectedValue + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                Response.Charset = "utf-8";
                Response.ContentType = "text/csv";
                
                // Add BOM for Excel to recognize UTF-8 properly
                Response.BinaryWrite(Encoding.UTF8.GetPreamble());
                Response.Output.Write(csv);
                Response.Flush();
                Response.End();
            }
        }

        private string GenerarCsv(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            // Cabeceras
            string[] columnNames = new string[dt.Columns.Count];
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                columnNames[i] = dt.Columns[i].ColumnName;
            }
            sb.AppendLine(string.Join(",", columnNames));

            // Filas
            foreach (DataRow row in dt.Rows)
            {
                string[] fields = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string field = row[i].ToString();
                    // Escapar comillas dobles y envolver en comillas si contiene comas
                    if (field.Contains(",") || field.Contains("\""))
                    {
                        field = "\"" + field.Replace("\"", "\"\"") + "\"";
                    }
                    fields[i] = field;
                }
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }
    }
}
