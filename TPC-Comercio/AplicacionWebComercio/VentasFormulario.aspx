<%@ Page Title="Nueva Venta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VentasFormulario.aspx.cs" Inherits="AplicacionWebComercio.VentasFormulario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">Nueva Venta</h2>
        <a href="Ventas.aspx" class="btn btn-outline-secondary btn-sm">← Volver al listado</a>
    </div>

    <div class="card">
        <div class="card-header">
            <h5 class="mb-0">Datos de la venta</h5>
        </div>
        <div class="card-body">

            <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-none w-100 mb-3" />

            <%-- Cliente --%>
            <div class="mb-3 row align-items-center">
                <label class="col-sm-2 col-form-label fw-semibold">Cliente</label>
                <div class="col-sm-5">
                    <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select" />
                </div>
            </div>

            <hr />

            <%-- Agregar ítem --%>
            <h6 class="mb-3">Agregar productos</h6>
            <div class="row g-2 mb-2 align-items-end">
                <div class="col-md-5">
                    <label class="form-label">Producto</label>
                    <asp:DropDownList ID="ddlProducto" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-2">
                    <label class="form-label">Cantidad</label>
                    <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" min="1"
                        ValidationGroup="AddItemVenta" placeholder="0" />
                    <asp:RequiredFieldValidator ID="rfvCantidadVenta" runat="server" ControlToValidate="txtCantidad"
                        ErrorMessage="*" CssClass="text-danger" ValidationGroup="AddItemVenta" Display="Dynamic" />
                    <asp:RangeValidator ID="rvCantidadVenta" runat="server" ControlToValidate="txtCantidad"
                        MinimumValue="1" MaximumValue="999999" Type="Integer" ErrorMessage="Inválida"
                        CssClass="text-danger" ValidationGroup="AddItemVenta" Display="Dynamic" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnAgregarItem" runat="server" Text="+ Agregar" CssClass="btn btn-secondary w-100"
                        OnClick="btnAgregarItem_Click" CausesValidation="true" ValidationGroup="AddItemVenta" />
                </div>
            </div>

            <div class="alert alert-info py-2 small mb-3">
                El precio final se calcula automáticamente con el costo del lote más antiguo disponible (FIFO) más el porcentaje de ganancia del producto.
            </div>

            <%-- Items en carrito --%>
            <asp:GridView ID="dgvItems" runat="server" AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm mt-2" EmptyDataText="Sin productos agregados."
                OnRowCommand="dgvItems_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Producto">
                        <ItemTemplate><%# Eval("Producto.NombreProducto") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Cantidad"           HeaderText="Cant."     ItemStyle-Width="70" />
                    <asp:BoundField DataField="PorcentajeGanancia" HeaderText="% Ganancia" DataFormatString="{0:0.##}%" HtmlEncode="false" ItemStyle-Width="100" />
                    <asp:TemplateField ItemStyle-Width="70">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkQuitar" runat="server" Text="Quitar"
                                CommandName="Quitar" CommandArgument='<%# Container.DataItemIndex %>'
                                OnClientClick="return confirm('¿Quitar este producto?');"
                                CssClass="btn btn-sm btn-outline-danger" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="d-flex justify-content-end mt-3">
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary me-2"
                    OnClick="btnCancelar_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardar" runat="server" Text="Confirmar y generar factura" CssClass="btn btn-primary"
                    OnClick="btnGuardar_Click" />
            </div>

        </div>
    </div>

</asp:Content>
