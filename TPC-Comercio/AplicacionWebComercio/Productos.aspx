<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="AplicacionWebComercio.Productos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Listado de Productos</h2>

    <asp:GridView ID="dgvProductos" runat="server" AutoGenerateColumns="true" CssClass="table">
    </asp:GridView>

</asp:Content>
