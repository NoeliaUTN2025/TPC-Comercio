<%@ Page Title="Confirmación de venta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConfirmacionVenta.aspx.cs" Inherits="AplicacionWebComercio.ConfirmacionVenta" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">Venta confirmada</h2>
    </div>

    <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-none w-100 mb-3" />

    <asp:Panel ID="pnlResumen" runat="server">
        <div class="card mb-3">
            <div class="card-header">
                <h5 class="mb-0">Datos de la venta</h5>
            </div>
            <div class="card-body">
                <p class="mb-1"><strong>Factura N°:</strong> <asp:Literal ID="litNumeroFactura" runat="server" /></p>
                <p class="mb-1"><strong>Fecha:</strong> <asp:Literal ID="litFecha" runat="server" /></p>
                <p class="mb-1"><strong>Cliente:</strong> <asp:Literal ID="litCliente" runat="server" /></p>
                <p class="mb-0"><strong>Forma de pago:</strong> <asp:Literal ID="litFormaPago" runat="server" /></p>
            </div>
        </div>

        <div class="card mb-3">
            <div class="card-header">
                <h5 class="mb-0">Productos</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="dgvItems" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm">
                    <Columns>
                        <asp:BoundField DataField="Producto.NombreProducto" HeaderText="Descripción" />
                        <asp:BoundField DataField="Cantidad"    HeaderText="Cant."       ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="PrecioVenta" HeaderText="P. Unitario" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="Subtotal"    HeaderText="Subtotal"    DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    </Columns>
                </asp:GridView>
                <div class="text-end fs-5">
                    Total: <strong><asp:Literal ID="litTotal" runat="server" /></strong>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlCuotas" runat="server" Visible="false" CssClass="card mb-3">
            <div class="card-header">
                <h5 class="mb-0">Plan de cuotas</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="dgvCuotas" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm">
                    <Columns>
                        <asp:BoundField DataField="NroCuota"    HeaderText="Cuota N°" />
                        <asp:BoundField DataField="Monto"       HeaderText="Monto"       DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="Interes"     HeaderText="Interés"     DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                        <asp:BoundField DataField="Vencimiento" HeaderText="Vencimiento" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <div class="d-flex justify-content-end gap-2">
            <a runat="server" id="lnkVerFactura" class="btn btn-outline-secondary">Ver / Imprimir factura</a>
            <a href="VentasFormulario.aspx" class="btn btn-outline-secondary">Nueva venta</a>
            <a href="Ventas.aspx" class="btn btn-primary">Volver al listado</a>
        </div>
    </asp:Panel>

</asp:Content>
