<%@ Page Title="Marcas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Marcas.aspx.cs" Inherits="AplicacionWebComercio.Marcas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Listado de Marcas</h2>
        <hr />
        <asp:GridView ID="dgvMarcas" runat="server" AutoGenerateColumns="true" CssClass="table table-striped table-bordered table-hover">
        </asp:GridView>
    </div>
</asp:Content>
