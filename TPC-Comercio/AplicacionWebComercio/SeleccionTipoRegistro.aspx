<%@ Page Title="Crear Cuenta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SeleccionTipoRegistro.aspx.cs" Inherits="AplicacionWebComercio.SeleccionTipoRegistro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <div class="text-center mb-4">
                    <h2>Crear Cuenta</h2>
                    <p class="text-muted">Seleccioná el tipo de cuenta que querés crear</p>
                </div>

                <div class="row">
                    <div class="col-md-6 mb-3">
                        <div class="card h-100 shadow-sm">
                            <div class="card-body text-center p-4">
                                <h4 class="card-title">👤 Cliente</h4>
                                <p class="card-text text-muted">
                                    Comprá productos con precio de venta final. Accedé a tu catálogo personalizado.
                                </p>
                                <a href="RegistroCliente.aspx" class="btn btn-primary btn-lg w-100">
                                    Registrarme como Cliente
                                </a>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-6 mb-3">
                        <div class="card h-100 shadow-sm">
                            <div class="card-body text-center p-4">
                                <h4 class="card-title">🏭 Proveedor</h4>
                                <p class="card-text text-muted">
                                    Ofrecé lotes de productos al negocio. Gestioná tus propuestas y consultá tus ventas.
                                </p>
                                <a href="RegistroProveedor.aspx" class="btn btn-success btn-lg w-100">
                                    Registrarme como Proveedor
                                </a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="text-center mt-3">
                    <a href="Login.aspx" class="text-decoration-none">← Volver al Login</a>
                </div>

            </div>
        </div>
    </div>

</asp:Content>
