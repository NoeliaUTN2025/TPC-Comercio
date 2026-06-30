<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CambiarContrasena.aspx.cs" Inherits="AplicacionWebComercio.CambiarContrasena" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Cambiar contraseña</h2>

    <div class="mb-3">
                <asp:Label ID="lblActual" runat="server" CssClass="form-label" Text="Contraseña actual"></asp:Label>
                <asp:TextBox ID="txtActual" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        
    </div>

    <div class="mb-3">
                <asp:Label ID="lblNueva" runat="server" CssClass="form-label" Text="Nueva contraseña"></asp:Label>
                <asp:TextBox ID="txtNueva" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
    </div>

    <div class="mb-3">
                <asp:Label ID="lblConfirmar" runat="server" CssClass="form-label" Text="Confirmar nueva contraseña"></asp:Label>
                <asp:TextBox ID="txtConfirmar" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
    </div>
    
    <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="btnGuardar_Click" />

</asp:Content>