<%@ Page Title="Catálogo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CatalogoProductos.aspx.cs" Inherits="AplicacionWebComercio.CatalogoProductos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4">
        <h2>Catálogo de Productos</h2>
        <p class="text-muted">Seleccioná los productos que querés comprar</p>

        <asp:Label runat="server" ID="lblMensaje" CssClass="alert d-none w-100 mb-3" />

        <%-- Selección de producto --%>
        <div class="card mb-4">
            <div class="card-body">
                <div class="row g-3 align-items-end">
                    <div class="col-md-5">
                        <label class="form-label">Producto</label>
                        <asp:DropDownList runat="server" ID="ddlProducto" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlProducto_SelectedIndexChanged" />
                    </div>
                    <div class="col-md-2">
                        <label class="form-label">Precio Unitario</label>
                        <asp:Label runat="server" ID="lblPrecioUnitario" CssClass="form-control-plaintext fw-bold text-success" Text="$0,00" />
                    </div>
                    <div class="col-md-2">
                        <label class="form-label">Cantidad</label>
                        <asp:TextBox runat="server" ID="txtCantidad" CssClass="form-control" Text="1" />
                    </div>
                    <div class="col-md-3">
                        <asp:Button runat="server" ID="btnAgregarItem" Text="Agregar al carrito" CssClass="btn btn-primary w-100" OnClick="btnAgregarItem_Click" />
                    </div>
                </div>
            </div>
        </div>

        <%-- Carrito --%>
        <h5>Mi carrito</h5>
        <asp:GridView runat="server" ID="dgvItems" CssClass="table table-bordered table-striped" AutoGenerateColumns="false"
            EmptyDataText="El carrito está vacío" OnRowCommand="dgvItems_RowCommand">
            <Columns>
                <asp:BoundField DataField="NombreProducto"   HeaderText="Producto"          ItemStyle-CssClass="align-middle" />
                <asp:BoundField DataField="Cantidad"          HeaderText="Cantidad"          ItemStyle-CssClass="align-middle" />
                <asp:BoundField DataField="PrecioVenta"       HeaderText="Precio Unit."      DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="align-middle" />
                <asp:BoundField DataField="Subtotal"          HeaderText="Subtotal"          DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="align-middle" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Quitar" CommandArgument='<%# Container.DataItemIndex %>'
                            CssClass="btn btn-sm btn-outline-danger"
                            OnClientClick="return confirm('¿Quitar este producto del carrito?');">Quitar</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="d-flex justify-content-between align-items-center mt-3">
            <h5>Total: <strong class="text-success"><asp:Literal runat="server" ID="litTotal" Text="$0,00" /></strong></h5>
            <div>
                <asp:Button runat="server" ID="btnCancelar" Text="Vaciar carrito" CssClass="btn btn-outline-secondary me-2" OnClick="btnCancelar_Click" />
                <asp:Button runat="server" ID="btnConfirmar" Text="Confirmar compra" CssClass="btn btn-success" OnClick="btnConfirmar_Click" />
            </div>
        </div>

    </div>

</asp:Content>
