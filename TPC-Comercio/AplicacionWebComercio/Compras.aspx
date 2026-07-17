<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Compras.aspx.cs"
    Inherits="AplicacionWebComercio.Compras" %>
<%@ Register Src="~/BarraFiltros.ascx" TagPrefix="uc1" TagName="BarraFiltros" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <h2>Compras</h2>

        <a href="ComprasFormulario.aspx" class="btn btn-success mb-3">Nueva Compra</a>

        <uc1:BarraFiltros runat="server" ID="ctrlFiltros" OnFiltrar="ctrlFiltros_Filtrar" MostrarCategoria="false" MostrarMarca="false" MostrarFechas="true" />

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block w-100 mb-3" />

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
                <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}" HtmlEncode="false" />
                <asp:TemplateField HeaderText="" ItemStyle-Width="100">
                    <ItemTemplate>
                        <a href='<%# "CompraReporte.aspx?id=" + Eval("Id") %>' class="btn btn-sm btn-outline-secondary">🖨 Reimprimir</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </asp:Content>