using System;
using System.Web.UI.WebControls;
using Negocio;
using Dominio;

namespace AplicacionWebComercio
{
    public partial class Marcas : System.Web.UI.Page
    {
        private const int PageSize = 5; // Tamaño de página para la paginación

        private int PageNumber
        {
            get 
            {
                return ViewState["PageNumber"] == null ? 1 : (int)ViewState["PageNumber"];
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
            if (!(Seguridad.EsAdmin(usuario) || Seguridad.EsVendedor(usuario) ))
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
            MarcaNegocio negocio = new MarcaNegocio();

            dgvMarcas.DataSource = negocio.ListarPaginado(PageNumber, PageSize, out totalRegistros);
           // dgvMarcas.DataSource = new MarcaNegocio().Listar();
            dgvMarcas.DataBind();

            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / PageSize);
            lblPagina.Text = $"Página {PageNumber} de {totalPaginas}";

            btnAnterior.Enabled = PageNumber > 1;
            btnSiguiente.Enabled = PageNumber < totalPaginas;
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
             if (PageNumber > 1)
                PageNumber--;

                CargarGrilla();
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
                PageNumber++;
                CargarGrilla();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Marca m = new Marca { Descripcion = txtDescripcion.Text.Trim() };
            MarcaNegocio negocio = new MarcaNegocio();

            if (hfId.Value == "0")
                negocio.Agregar(m);
            else
            {
                m.Id = int.Parse(hfId.Value);
                negocio.Modificar(m);
            }

            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvMarcas.EditIndex = -1;
            pnlFormulario.Visible = false;
            CargarGrilla();
        }

        protected void dgvMarcas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Editar") return;

            int id = int.Parse(e.CommandArgument.ToString());
            Marca m = new MarcaNegocio().Listar().Find(x => x.Id == id);

            hfId.Value = m.Id.ToString();
            txtDescripcion.Text = m.Descripcion;

            pnlFormulario.Visible = true;
            CargarGrilla();
        }

        protected void dgvMarcas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = (int)dgvMarcas.DataKeys[e.RowIndex].Value;
                new MarcaNegocio().Eliminar(id);
                lblError.Visible = false;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
            finally
            {
                CargarGrilla();
            }
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "0";
            txtDescripcion.Text = "";
        }
    }
}
