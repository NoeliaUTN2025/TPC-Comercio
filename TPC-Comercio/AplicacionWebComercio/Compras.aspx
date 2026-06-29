<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Compras.aspx.cs"
    Inherits="AplicacionWebComercio.Compras" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <h2>Compras</h2>

        <a href="ComprasFormulario.aspx" class="btn btn-success mb-3">Nueva Compra</a>

        <asp:GridView ID="dgvCompras" runat="server" AutoGenerateColumns="false" CssClass="table table-striped">
            <Columns>
                <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}"
                    HtmlEncode="false" />
                <asp:TemplateField HeaderText="Proveedor">
                    <ItemTemplate>
                        <%# Eval("Proveedor.RazonSocial") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CantidadTotal" HeaderText="Cant. Comprada" />
                <asp:BoundField DataField="CodigosProductos" HeaderText="Códigos del Producto" />
                <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" HtmlEncode="false" />
            </Columns>
        </asp:GridView>

    </asp:Content>