using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dominio;
using Negocio;

namespace AplicacionWebComercio
{
    public partial class MisCompras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario u = Session["Usuario"] as Usuario;
            if (!Seguridad.EsProveedor(u))
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                try
                {
                    CargarProductos();
                    CargarGrillas();
                }
                catch (Exception ex)
                {
                    MostrarError("Error al cargar los datos: " + ex.Message);
                }
            }
        }

        private int IdProveedor => ((Usuario)Session["Usuario"]).IdEntidad;

        private void CargarProductos()
        {
            ddlProducto.DataSource     = new ProductoNegocio().Listar();
            ddlProducto.DataTextField  = "NombreProducto";
            ddlProducto.DataValueField = "Id";
            ddlProducto.DataBind();
        }

        private void CargarGrillas()
        {
            // Compras del negocio a este proveedor
            var compras = new CompraNegocio().ListarPorProveedor(IdProveedor);
            dgvMisCompras.DataSource = compras;
            dgvMisCompras.DataBind();

            // Propuestas enviadas por este proveedor
            var propuestas = new PropuestaNegocio().ListarPorProveedor(IdProveedor);
            dgvPropuestas.DataSource = propuestas.Select(p => new {
                p.Id,
                NombreProducto = p.Producto.NombreProducto,
                p.Cantidad,
                p.PrecioUnitario,
                p.Estado,
                p.Fecha
            }).ToList();
            dgvPropuestas.DataBind();
        }

        protected void btnProponer_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarError("Ingresá una cantidad válida.");
                return;
            }
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio <= 0)
            {
                MostrarError("Ingresá un precio unitario válido.");
                return;
            }

            try
            {
                new PropuestaNegocio().Insertar(new PropuestaProveedor
                {
                    Proveedor      = new Proveedor { ID = IdProveedor },
                    Producto       = new Producto  { Id = int.Parse(ddlProducto.SelectedValue) },
                    Cantidad       = cantidad,
                    PrecioUnitario = precio
                });

                MostrarExito("Propuesta enviada correctamente. El administrador la revisará a la brevedad.");
                txtCantidad.Text = "";
                txtPrecio.Text   = "";
                CargarGrillas();
            }
            catch (Exception ex)
            {
                MostrarError("Error al enviar la propuesta: " + ex.Message);
            }
        }

        private void MostrarError(string msg)
        {
            lblMensaje.Text = msg;
            lblMensaje.CssClass = "alert alert-danger d-block w-100 mb-3";
        }

        private void MostrarExito(string msg)
        {
            lblMensaje.Text = msg;
            lblMensaje.CssClass = "alert alert-success d-block w-100 mb-3";
        }
    }
}
