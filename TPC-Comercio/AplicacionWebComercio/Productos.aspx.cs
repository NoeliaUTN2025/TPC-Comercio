using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;
using System.IO;

namespace AplicacionWebComercio
{
    public partial class Productos : System.Web.UI.Page
    {
        private const int PageSize = 5; // Tamaño de página para la paginación

         private int PageNumber
         {
             get
             {
                 return (ViewState["PageNumber"] == null? 1 : (int)ViewState["PageNumber"]);
             }
             set
             {
                 ViewState["PageNumber"] = value;
             }
         }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Seguridad.SesionActiva((Session)["Usuario"]))
            {
                Response.Redirect("Login.aspx", false);
                return;
            }

            Usuario usuario = (Usuario)Session["Usuario"];
            if (!(Seguridad.EsAdmin(usuario) || Seguridad.EsVendedor(usuario) || Seguridad.EsCliente(usuario)))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No tiene permisos para acceder a esta sección.'); window.location='Default.aspx';", true);
                return;
            }

            if (!IsPostBack)
                CargarGrilla();


        }

        private void CargarGrilla()
        {
            int totalRegistros;

            ProductoNegocio negocio = new ProductoNegocio();
            dgvProductos.DataSource = negocio.ListarPaginado(PageNumber, PageSize, out totalRegistros);
            //dgvProductos.DataSource = new ProductoNegocio().Listar(); 
            dgvProductos.DataBind();

            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / PageSize);

            lblPagina.Text = "Página " + PageNumber + " de "  + totalPaginas;

            btnAnterior.Enabled = PageNumber > 1;
            btnSiguiente.Enabled = PageNumber < totalPaginas; 
        }

        protected void btnAnterior_Click(Object sender, EventArgs e)
        {
            if (PageNumber > 1)
                PageNumber--;

            CargarGrilla(); 
        }

        protected void btnSiguiente_Click(Object sendender, EventArgs e)
        {
            PageNumber++; 
         

            CargarGrilla();
        }


        private void CargarDropDowns()
        {
            ddlMarca.DataSource = new MarcaNegocio().Listar();
            ddlMarca.DataBind();

            ddlCategoria.DataSource = new CategoriaNegocio().Listar();
            ddlCategoria.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            CargarDropDowns();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio) || precio < 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El precio ingresado no es válido.');", true);
                return;
            }
            if (!int.TryParse(txtStockMinimo.Text, out int stockMinimo) || stockMinimo < 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El stock mínimo ingresado no es válido.');", true);
                return;
            }
            if (!decimal.TryParse(txtPorcentajeGanancia.Text, out decimal porcentaje) || porcentaje < 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('El porcentaje de ganancia ingresado no es válido.');", true);
                return;
            }

            try
            {
             

                Producto p = new Producto();
                p.Codigo = txtCodigo.Text.Trim();
                p.NombreProducto = txtNombre.Text.Trim();
                p.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim();
                p.Precio = precio;
                p.StockMinimo = stockMinimo;
                p.PorcentajeGanancia = porcentaje;
                p.marca = new Marca { Id = int.Parse(ddlMarca.SelectedValue) };
                p.categoria = new Categoria { Id = int.Parse(ddlCategoria.SelectedValue) };

                if (txtImagen.PostedFile !=null && txtImagen.PostedFile.ContentLength > 0)
                {
                    string nombreArchivo = Path.GetFileName(txtImagen.PostedFile.FileName);
                    string carpeta = Server.MapPath("~/Images/Productos/");

                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }
                    string rutaServidor = Path.Combine(carpeta, nombreArchivo);
                    txtImagen.PostedFile.SaveAs(rutaServidor);
                    p.UrlImagen = "~/Images/Productos/" + nombreArchivo;
                }

                ProductoNegocio negocio = new ProductoNegocio();

                if (hfId.Value == "0")
                    negocio.Agregar(p);
                else
                {
                    p.Id = int.Parse(hfId.Value);
                    negocio.Modificar(p);
                }

                pnlFormulario.Visible = false;
            }
            catch (Exception ex)
            {
                string msg = System.Web.HttpUtility.JavaScriptStringEncode("Error al guardar el producto: " + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + msg + "');", true);
                return;
            }
            CargarGrilla();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvProductos.EditIndex = -1;
            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void dgvProductos_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Producto p = new ProductoNegocio().Listar().Find(x => x.Id == id);

            hfId.Value = p.Id.ToString();
            txtCodigo.Text = p.Codigo;
            txtNombre.Text = p.NombreProducto;
            txtDescripcion.Text = p.Descripcion;
            imgProducto.ImageUrl = p.UrlImagen;
            txtPrecio.Text = p.Precio.ToString();
            txtStockMinimo.Text = p.StockMinimo.ToString();
            txtPorcentajeGanancia.Text = p.PorcentajeGanancia.ToString();

            CargarDropDowns();
            ddlMarca.SelectedValue = p.marca.Id.ToString();
            ddlCategoria.SelectedValue = p.categoria.Id.ToString();

            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = (int)dgvProductos.DataKeys[e.RowIndex].Value;
                new ProductoNegocio().EliminarLogico(id);
            }
            catch (Exception ex)
            {
                string msg = System.Web.HttpUtility.JavaScriptStringEncode("Error al eliminar el producto: " + ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + msg + "');", true);
                return;
            }
            CargarGrilla();
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "0";
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            imgProducto.ImageUrl = "~/Image/sin-image.png";
            txtPrecio.Text = "";
            txtStockMinimo.Text = "";
            txtPorcentajeGanancia.Text = "";
        }
    }
}
