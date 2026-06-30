<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AplicacionWebComercio.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class ="container mt-5">
        <div class ="row justify-content-center">
            <div class ="col-md-5">
                <div class="card shadow">

                    <div class="card-header text-center">
                        <h3>Sistema de Gestión Comerial</h3>
                        <h5>Iniciar Sesión</h5>
                        </div>

                    <div class="card-body">
                        <div class="mb-3">
                            <label class="form-label">Usuario</label>

                            <asp:TextBox runat= "server" ID="txtUsuario" CssClass="form-control" placeholder="Ingrese su usuario">
                            </asp:TextBox>
                             
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">Contraseña</label>
                            <asp:TextBox runat= "server" ID="txtPassword" CssClass="form-control" TextMode="Password" placeholder="Ingrese su contraseña">
                                </asp:TextBox>
                            </div>

                        <div class="d-grid">
                            <asp:Button runat="server" ID="btnLogin" Text="Iniciar Sesión" CssClass="btn btn-primary btn-lg" OnClick="btnLogin_Click" />
                            </div>
                        <br />
                        <asp:Label runat="server" ID="lblError" CssClass="text-danger fx-bold">

                        </asp:Label>

                        </div>
    
            
                    </div> 

                </div>

            </div> 

        </div> 

  
   

</asp:Content>
