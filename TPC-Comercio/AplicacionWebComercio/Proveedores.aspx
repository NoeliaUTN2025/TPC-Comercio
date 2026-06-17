<%@ Page Title="Proveedores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Proveedores.aspx.cs" Inherits="AplicacionWebComercio.Proveedores" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Listado de Proveedores</h2>
        <hr />
        <asp:GridView ID="dgvProveedores" runat="server" AutoGenerateColumns="true" CssClass="table table-striped table-bordered table-hover">
        </asp:GridView>
    </div>
</asp:Content>
