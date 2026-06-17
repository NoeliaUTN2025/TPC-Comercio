<%@ Page Title="Categorias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Categorias.aspx.cs" Inherits="AplicacionWebComercio.Categorias" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Listado de Categorias</h2>
        <hr />
        <asp:GridView ID="dgvCategorias" runat="server" AutoGenerateColumns="true" CssClass="table table-striped table-bordered table-hover">
        </asp:GridView>
    </div>
</asp:Content>
