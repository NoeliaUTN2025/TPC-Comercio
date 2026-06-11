<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="AplicacionWebComercio.Clientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   
    <h2>Listado de Clientes</h2>

    <asp:GridView ID="dgvClientes" runat="server" AutoGenerateColumns="true" CssClass="table">
    </asp:GridView> 

</asp:Content>
