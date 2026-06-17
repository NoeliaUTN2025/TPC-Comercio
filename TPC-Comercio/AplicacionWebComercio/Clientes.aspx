<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="AplicacionWebComercio.Clientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  


    <h2>Listado de Clientes</h2>

     <asp:Button ID="btnAgregar" runat="server" Text="Agregar Cliente" CssClass ="btn btn-primary" PostBackUrl="~/ClientesFormularios.aspx" />
     <asp:Button ID="btnEliminar" runat="server" Text="Eliminar Cliente" CssClass ="btn btn-danger" OnClick="btnEliminar_Click" /> 
    
    <br /><br />     
    
    <asp:GridView ID="dgvClientes" runat="server" AutoGenerateColumns="true" CssClass="table"
        AutoGenerateSelectButton="true" OnSelectedIndexChanged="dgvClientes_SelectedIndexChanged"> 
     
    </asp:GridView> 



</asp:Content>
