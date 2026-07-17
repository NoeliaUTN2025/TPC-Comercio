<%@ Page Title="Nueva Compra" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ComprasFormulario.aspx.cs" Inherits="AplicacionWebComercio.ComprasFormulario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">Nueva Compra</h2>
        <a href="Compras.aspx" class="btn btn-outline-secondary btn-sm">← Volver al listado</a>
    </div>

    <%-- Propuestas pendientes de proveedores --%>
    <asp:Panel runat="server" ID="pnlPropuestas" Visible="false" CssClass="mb-4">
        <div class="card border-warning">
            <div class="card-header bg-warning text-dark">
                <strong>Propuestas de proveedores pendientes de aprobación</strong>
            </div>
            <div class="card-body p-0">
                <asp:GridView runat="server" ID="dgvPropuestas" CssClass="table table-sm table-hover mb-0"
                    AutoGenerateColumns="false" EmptyDataText="Sin propuestas." OnRowCommand="dgvPropuestas_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="RazonSocial"    HeaderText="Proveedor" />
                        <asp:BoundField DataField="NombreProducto" HeaderText="Producto"  />
                        <asp:BoundField DataField="Cantidad"        HeaderText="Cant."    ItemStyle-Width="60" />
                        <asp:BoundField DataField="PrecioUnitario"  HeaderText="P. Unit."  DataFormatString="{0:C}" HtmlEncode="false" />
                        <asp:BoundField DataField="Fecha"           HeaderText="Fecha"    DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" ItemStyle-Width="90" />
                        <asp:TemplateField HeaderText="Acciones" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="text-center" ItemStyle-Width="190">
                            <ItemTemplate>
                                <div style="display:flex;gap:5px; justify-content:center; align-items:center";>
                                <asp:LinkButton runat="server" CommandName="Aprobar" CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-sm btn-success btn-sm"
                                    Style="width: 90px;"
                                    OnClientClick="return confirm('¿Aprobar esta propuesta y generar la orden de compra?');">✔ Aprobar</asp:LinkButton>
                           
                                <asp:LinkButton runat="server" CommandName="Rechazar" CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-sm btn-danger btn-sm"
                                    Style="width: 90px;"
                                    OnClientClick="return confirm('¿Desea Rechazar la propuesta?');">✖ Rechazar</asp:LinkButton>
                                     </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>

    <%-- Formulario de compra manual --%>
    <div class="card">
        <div class="card-header">
            <h5 class="mb-0">Compra manual</h5>
        </div>
        <div class="card-body">

            <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-none w-100 mb-3" />

            <%-- Proveedor --%>
            <div class="mb-3 row align-items-center">
                <label class="col-sm-2 col-form-label fw-semibold">Proveedor</label>
                <div class="col-sm-5">
                    <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select" />
                </div>
            </div>

            <hr />

            <%-- Agregar ítem --%>
            <h6 class="mb-3">Agregar productos</h6>
            <div class="row g-2 mb-2 align-items-end">
                <div class="col-md-4">
                    <label class="form-label">Producto</label>
                    <asp:DropDownList ID="ddlProducto" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-2">
                    <label class="form-label">Cantidad</label>
                    <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" min="1" ValidationGroup="AddItem" placeholder="0" />
                    <asp:RequiredFieldValidator ID="rfvCantidad" runat="server" ControlToValidate="txtCantidad" ErrorMessage="*" CssClass="text-danger" ValidationGroup="AddItem" Display="Dynamic" />
                    <asp:RangeValidator ID="rvCantidad" runat="server" ControlToValidate="txtCantidad" MinimumValue="1" MaximumValue="999999" Type="Integer" ErrorMessage="Inválida" CssClass="text-danger" ValidationGroup="AddItem" Display="Dynamic" />
                </div>
                <div class="col-md-2">
                    <label class="form-label">Precio unit. ($)</label>
                    <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" ValidationGroup="AddItem" placeholder="0,00" />
                    <asp:RequiredFieldValidator ID="rfvPrecio" runat="server" ControlToValidate="txtPrecioUnitario" ErrorMessage="*" CssClass="text-danger" ValidationGroup="AddItem" Display="Dynamic" />
                    <asp:RangeValidator ID="rvPrecio" runat="server" ControlToValidate="txtPrecioUnitario" MinimumValue="0" MaximumValue="99999999" Type="Double" ErrorMessage="Inválido" CssClass="text-danger" ValidationGroup="AddItem" Display="Dynamic" />
                </div>
                <div class="col-md-2">
                    <asp:Button ID="btnAgregarItem" runat="server" Text="+ Agregar" CssClass="btn btn-secondary w-100"
                        OnClick="btnAgregarItem_Click" CausesValidation="true" ValidationGroup="AddItem" />
                </div>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="text-danger small" Visible="false" />

            <%-- Items en carrito --%>
            <asp:GridView ID="dgvItems" runat="server" AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm mt-3" EmptyDataText="Sin productos agregados."
                OnRowCommand="dgvItems_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Producto">
                        <ItemTemplate><%# Eval("Producto.NombreProducto") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Cantidad"      HeaderText="Cant."      ItemStyle-Width="70" />
                    <asp:BoundField DataField="PrecioUnitario" HeaderText="P. Unit."  DataFormatString="{0:C}" HtmlEncode="false" />
                    <asp:BoundField DataField="Subtotal"      HeaderText="Subtotal"   DataFormatString="{0:C}" HtmlEncode="false" />
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

            <div class="d-flex justify-content-between align-items-center mt-3">
                <h5 class="mb-0">
                    Total: <asp:Label ID="lblTotal" runat="server" Text="$ 0,00" CssClass="text-success" />
                </h5>
                <div>
                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary me-2"
                        OnClick="btnCancelar_Click" CausesValidation="false" />
                    <asp:Button ID="btnGuardar" runat="server" Text="Registrar compra" CssClass="btn btn-primary"
                        OnClick="btnGuardar_Click" />
                </div>
            </div>

        </div>
    </div>

</asp:Content>
