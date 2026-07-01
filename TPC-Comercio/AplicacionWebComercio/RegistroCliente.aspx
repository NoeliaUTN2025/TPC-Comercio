<%@ Page Title="Registro Cliente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistroCliente.aspx.cs" Inherits="AplicacionWebComercio.RegistroCliente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4">
        <div class="row justify-content-center">
            <div class="col-md-7">
                <div class="card shadow">

                    <div class="card-header text-center">
                        <h4>Registro de Cliente</h4>
                    </div>

                    <div class="card-body">

                        <asp:Label runat="server" ID="lblMensaje" CssClass="alert d-none w-100 mb-3" />

                        <h6 class="text-muted mb-3">Datos personales</h6>

                        <div class="row mb-3">
                            <div class="col-md-4">
                                <label class="form-label">DNI <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtDni" CssClass="form-control" placeholder="Ej: 12345678" MaxLength="15" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Nombre <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control" MaxLength="100" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Apellido <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtApellido" CssClass="form-control" MaxLength="100" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <label class="form-label">Dirección</label>
                                <asp:TextBox runat="server" ID="txtDireccion" CssClass="form-control" MaxLength="200" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label class="form-label">Teléfono</label>
                                <asp:TextBox runat="server" ID="txtTelefono" CssClass="form-control" MaxLength="20" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Email</label>
                                <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" MaxLength="150" TextMode="Email" />
                            </div>
                        </div>

                        <hr />
                        <h6 class="text-muted mb-3">Credenciales de acceso</h6>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <label class="form-label">Nombre de usuario <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtUsuario" CssClass="form-control" MaxLength="50" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label class="form-label">Contraseña <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtContrasena" CssClass="form-control" TextMode="Password" MaxLength="50" />
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Confirmar contraseña <span class="text-danger">*</span></label>
                                <asp:TextBox runat="server" ID="txtConfirmar" CssClass="form-control" TextMode="Password" MaxLength="50" />
                            </div>
                        </div>

                        <div class="d-grid gap-2 d-md-flex justify-content-md-end mt-4">
                            <a href="SeleccionTipoRegistro.aspx" class="btn btn-outline-secondary me-md-2">Cancelar</a>
                            <asp:Button runat="server" ID="btnRegistrar" Text="Crear Cuenta" CssClass="btn btn-primary" OnClick="btnRegistrar_Click" />
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
