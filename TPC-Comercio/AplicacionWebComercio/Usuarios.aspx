<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="AplicacionWebComercio.Usuarios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Gestión de Usuarios</h2>
    
    <div class="mb-3"> 
        <asp:Label ID="lblUsuario" runat="server" Text="Usuario" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control"></asp:TextBox>
    </div>
    
    <div class="mb-3"> 
        <asp:Label ID="lblContraseña" runat="server" Text="Contraseña" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtContraseña" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
    </div>

    <div class="mb-3"> 
        <asp:Label ID="lblConfirmarContraseña" runat="server" Text="Confirmar Contraseña" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="txtConfirmarContraseña" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
    </div>

    <div class="mb-3"> 
        <asp:Label ID="lblIdPerfil" runat="server" Text="Perfil" CssClass="form-label"></asp:Label>
        <asp:DropDownList ID="ddlIdPerfil" runat="server" CssClass="form-control">
        <asp:ListItem Text="Administrador" Value="1"></asp:ListItem>
        <asp:ListItem Text="Vendedor" Value="2"></asp:ListItem>
        <asp:ListItem Text="Cliente" Value="3"></asp:ListItem>
        <asp:ListItem Text="Proveedor" Value="4"></asp:ListItem>
            </asp:DropDownList>
          
    </div> 

     <div class="mb-3"> 
         <asp:CheckBox ID="chkEstado" runat="server" Text="Activo" Checked="true" CssClass="form-label"></asp:CheckBox>
      
    </div> 

        <div class="mb-3"> 
        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary"></asp:Button>
     
   </div> 
      

         <div class="mb-3"> 
     <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="form-label"></asp:Button>
  
</div> 

</asp:Content>
