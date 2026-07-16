<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="AplicacionWebComercio.Productos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Gestión de Productos</h2>

    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo Producto" OnClick="btnNuevo_Click" CssClass="btn btn-success mb-3" />

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card p-3 mb-3">
        <h4>Datos del Producto</h4>
        <asp:HiddenField ID="hfId" runat="server" Value="0" />

        <div class="mb-2">
            <label>Código</label>
            <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Nombre</label>
            <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Descripción</label>
            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label class="form-label">Imagen</label>
            <input type="file" ID="txtImagen" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <asp:Image ID="imgProducto" runat="server" CssClass="img-thumbnail" Width="150px" Height="180px" ImageUrl="~/Images/sin-image.png" />
            
        </div>


        <div class="mb-2">
            <label>Precio de Costo</label>
            <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Stock Mínimo</label>
            <asp:TextBox ID="txtStockMinimo" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>% Ganancia</label>
            <asp:TextBox ID="txtPorcentajeGanancia" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Marca</label>
            <asp:DropDownList ID="ddlMarca" runat="server" CssClass="form-select"
                DataTextField="Descripcion" DataValueField="Id" />
        </div>
        <div class="mb-2">
            <label>Categoría</label>
            <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select"
                DataTextField="Descripcion" DataValueField="Id" />
        </div>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary me-2" />
        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="btn btn-secondary" CausesValidation="false" />
    </asp:Panel>

    <asp:GridView ID="dgvProductos" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
        DataKeyNames="Id" OnRowCommand="dgvProductos_RowCommand" OnRowDeleting="dgvProductos_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Codigo" HeaderText="Código" />
            <asp:BoundField DataField="NombreProducto" HeaderText="Nombre" />
            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
            <asp:BoundField DataField="StockActual" HeaderText="Stock" />
            <asp:BoundField DataField="StockMinimo" HeaderText="Stock Mín." />
            <asp:BoundField DataField="Precio" HeaderText="Precio Ref." DataFormatString="{0:C2}" />
            <asp:BoundField DataField="PorcentajeGanancia" HeaderText="% Ganancia" DataFormatString="{0:N2}" />
            <asp:TemplateField HeaderText="Marca">
                <ItemTemplate><%# Eval("marca.Descripcion") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Categoría">
                <ItemTemplate><%# Eval("categoria.Descripcion") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEditar" runat="server" Text="Editar"
                        CommandName="Editar" CommandArgument='<%# Eval("Id") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEliminar" runat="server" Text="Eliminar" CommandName="Delete"
                        OnClientClick="return confirm('¿Confirma la baja del producto?');" CssClass="text-danger" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <div class="d-flex justify-content-between align-items-center mt-3"> 
        <asp:Button ID="btnAnterior" runat="server" Text="Anterior" OnClick="btnAnterior_Click" CssClass="btn btn-secondary" />
        <asp:Label ID="lblPagina" runat="server" Text="Página 1 de 1" CssClass="fw-bold text-center" />
        <asp:Button ID="btnSiguiente" runat="server" Text="Siguiente" OnClick="btnSiguiente_Click" CssClass="btn btn-secondary" />
    </div>

</asp:Content>
