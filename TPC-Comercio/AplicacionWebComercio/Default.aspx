<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AplicacionWebComercio._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row text-center my-4" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle" class="display-4 fw-bold">Sistema de Gestión Comercial</h1>
            <p class="lead text-muted">Panel de Control Principal</p>
        </section>

        <asp:Panel ID="pnlDashboard" runat="server" Visible="false">
            <div class="row g-4 mt-4">

                <!-- Tarjeta Total Ventas -->
                <div class="col-md-4">
                    <div class="card text-white bg-success shadow h-100">
                        <div class="card-body d-flex flex-column justify-content-center align-items-center">
                            <h5 class="card-title text-uppercase fw-bold">Total Ventas (Mes)</h5>
                            <h2 class="display-5 fw-bold mt-2">
                                <asp:Literal ID="litTotalVentas" runat="server" Text="$0.00"></asp:Literal>
                            </h2>
                        </div>
                    </div>
                </div>

                <!-- Tarjeta Total Compras -->
                <div class="col-md-4">
                    <div class="card text-white bg-primary shadow h-100">
                        <div class="card-body d-flex flex-column justify-content-center align-items-center">
                            <h5 class="card-title text-uppercase fw-bold">Total Compras (Mes)</h5>
                            <h2 class="display-5 fw-bold mt-2">
                                <asp:Literal ID="litTotalCompras" runat="server" Text="$0.00"></asp:Literal>
                            </h2>
                        </div>
                    </div>
                </div>

                <!-- Tarjeta Alerta Stock -->
                <div class="col-md-4">
                    <div class="card text-white bg-danger shadow h-100">
                        <div class="card-body d-flex flex-column justify-content-center align-items-center">
                            <h5 class="card-title text-uppercase fw-bold">Alertas de Stock</h5>
                            <h2 class="display-5 fw-bold mt-2">
                                <asp:Literal ID="litBajoStock" runat="server" Text="0"></asp:Literal>
                            </h2>
                            <p class="card-text mb-0 mt-2">Productos bajo el mínimo</p>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlMensajeAnonimo" runat="server" Visible="true" CssClass="text-center mt-5">
            <h4>Bienvenido. Por favor, inicia sesión para acceder a las opciones del sistema.</h4>
        </asp:Panel>

    </main>

</asp:Content> 
