<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClientesFormularios.aspx.cs" Inherits="AplicacionWebComercio.ClientesFormularios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2> Alta/Modificacion Cliente</h2> 

    <div class="mb-3"> 
        <asp:Label ID="lblDNI" runat="server" Text="DNI" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtDNI" runat="server" CssClass="form-control"></asp:TextBox>
    </div>
    
    <div class="mb-3"> 
        <asp:Label ID="lblNombre" runat="server" Text="Nombre" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"></asp:TextBox>
    </div>

    <div class="mb-3"> 
        <asp:Label ID="lblApellido" runat="server" Text="Apellido" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control"></asp:TextBox>
    </div>

    <div class="mb-3"> 
        <asp:Label ID="lblDireccion" runat="server" Text="Direccion" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control"></asp:TextBox>
    </div> 
    
    <div class="mb-3"> 
        <asp:Label ID="lblTelefono" runat="server" Text="Telefono" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"></asp:TextBox>
    </div> 

    <div class="mb-3">
        <asp:Label ID="lblEmail" runat="server" Text="Email" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ></asp:TextBox>
    </div>
        
  
    <div class="mb-3">
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary"></asp:Button>
        <a href="Clientes.aspx" class="btn btn-secondary">Cancelar</a>

    </div>

        <div class="mb-3">
        <asp:CheckBox ID="chkConfirmarEliminacion" runat="server" Text="Confirmar Eliminacion" ></asp:CheckBox>
    </div>

    <div class="mb-3">
        <asp:Button ID="btnEliminar" runat="server" Text="Eliminar Cliente" CssClass ="btn btn-danger" OnClick="btnEliminar_Click" /> 
    </div>

     <br />

</asp:Content>
