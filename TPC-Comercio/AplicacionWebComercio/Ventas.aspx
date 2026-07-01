<%@ Page Title="Ventas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ventas.aspx.cs" Inherits="AplicacionWebComercio.Ventas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Listado de Ventas</h2>
        <hr />
        
        <a href="VentasFormulario.aspx" class="btn btn-success mb-3">Nueva Venta</a>

        <asp:GridView ID="dgvVentas" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered">
            <Columns>
                <asp:BoundField DataField="NumeroFactura" HeaderText="Factura N°" />
                <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Cliente">
                    <ItemTemplate>
                        <%# Eval("Cliente.Nombre") %> <%# Eval("Cliente.Apellido") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" />
                <asp:TemplateField HeaderText="" ItemStyle-Width="100">
                    <ItemTemplate>
                        <a href='<%# "FacturaReporte.aspx?id=" + Eval("Id") %>' class="btn btn-sm btn-outline-secondary">🖨 Reimprimir</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
