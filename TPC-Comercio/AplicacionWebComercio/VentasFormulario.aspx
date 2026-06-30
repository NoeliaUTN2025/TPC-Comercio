<%@ Page Title="Nueva Venta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VentasFormulario.aspx.cs" Inherits="AplicacionWebComercio.VentasFormulario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Registrar Nueva Venta</h2>
        <hr />

        <div class="mb-3">
            <label class="form-label fw-bold">Cliente</label>
            <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select" />
        </div>

        <h4 class="mt-4">Agregar productos</h4>
        <div class="row mb-3 align-items-end">
            <div class="col-md-5">
                <label class="form-label fw-bold">Producto</label>
                <asp:DropDownList ID="ddlProducto" runat="server" CssClass="form-select" />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-bold">Cantidad</label>
                <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" min="1" ValidationGroup="AddItemVenta" />
                <asp:RequiredFieldValidator ID="rfvCantidadVenta" runat="server" ControlToValidate="txtCantidad" ErrorMessage="*" CssClass="text-danger" ValidationGroup="AddItemVenta" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RangeValidator ID="rvCantidadVenta" runat="server" ControlToValidate="txtCantidad" MinimumValue="1" MaximumValue="999999" Type="Integer" ErrorMessage="Inválida" CssClass="text-danger" ValidationGroup="AddItemVenta" Display="Dynamic"></asp:RangeValidator>
            </div>
            <div class="col-md-4">
                <asp:Button ID="btnAgregarItem" runat="server" Text="Agregar al Carrito" CssClass="btn btn-secondary w-100" OnClick="btnAgregarItem_Click" CausesValidation="true" ValidationGroup="AddItemVenta" />
            </div>
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="mt-2 d-block fw-bold" />

        <asp:GridView ID="dgvItems" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered mt-4" OnRowCommand="dgvItems_RowCommand">
            <Columns>
                <asp:BoundField DataField="Producto.NombreProducto" HeaderText="Producto" />
                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad Solicitada" />
                <asp:BoundField DataField="PorcentajeGanancia" HeaderText="% Ganancia a Aplicar" DataFormatString="{0:0.##}%" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkQuitar" runat="server" Text="Quitar"
                            CommandName="Quitar" CommandArgument='<%# Container.DataItemIndex %>'
                            OnClientClick="return confirm('¿Quitar este producto?');" CssClass="btn btn-danger btn-sm" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        
        <div class="alert alert-info mt-3">
            <i class="bi bi-info-circle"></i> El precio final de cada unidad será calculado automáticamente usando el costo de compra del Lote más antiguo disponible, sumándole el porcentaje de ganancia del producto.
        </div>

        <div class="mt-4 text-end">
            <asp:Button ID="btnGuardar" runat="server" Text="Confirmar y Generar Factura" CssClass="btn btn-primary btn-lg me-2" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary btn-lg" OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </div>
</asp:Content>
