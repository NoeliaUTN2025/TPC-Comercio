<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ComprasFormulario.aspx.cs" Inherits="AplicacionWebComercio.ComprasFormulario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Nueva Compra</h2>

    <div class="mb-3">
        <label>Proveedor</label>
        <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-control" />
    </div>

    <hr />

    <h4>Agregar productos</h4>

    <div class="row mb-3">
        <div class="col-md-4">
            <label>Producto</label>
            <asp:DropDownList ID="ddlProducto" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-2">
            <label>Cantidad</label>
            <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-2">
            <label>Precio Unitario</label>
            <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-2 align-self-end">
            <asp:Button ID="btnAgregarItem" runat="server" Text="Agregar" CssClass="btn btn-secondary" OnClick="btnAgregarItem_Click" CausesValidation="false" />
        </div>
    </div>

    <asp:Label ID="lblError" runat="server" CssClass="text-danger" Visible="false" />

    <asp:GridView ID="dgvItems" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
        OnRowCommand="dgvItems_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="Producto">
                <ItemTemplate><%# Eval("Producto.NombreProducto") %></ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
            <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unitario" DataFormatString="{0:C}" HtmlEncode="false" />
            <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:C}" HtmlEncode="false" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkQuitar" runat="server" Text="Quitar"
                        CommandName="Quitar" CommandArgument='<%# Container.DataItemIndex %>'
                        OnClientClick="return confirm('¿Quitar este producto?');" CssClass="text-danger" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <div class="mb-3">
        <strong>Total: </strong>
        <asp:Label ID="lblTotal" runat="server" Text="$0,00" />
    </div>

    <asp:Button ID="btnGuardar" runat="server" Text="Guardar Compra" CssClass="btn btn-primary me-2" OnClick="btnGuardar_Click" />
    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" CausesValidation="false" />

</asp:Content>
