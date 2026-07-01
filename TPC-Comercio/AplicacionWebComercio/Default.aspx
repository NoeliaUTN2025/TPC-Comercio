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
        
        <asp:Panel ID="pnlActividadReciente" runat="server" Visible="false" CssClass="mt-5">
            <h3 class="mb-4 text-secondary border-bottom pb-2">Tu Actividad Reciente</h3>
            <div class="row">
                <asp:Repeater ID="rptActividad" runat="server">
                    <ItemTemplate>
                        <div class="col-md-4 mb-3">
                            <div class="card shadow-sm border-0 h-100">
                                <div class="card-body">
                                    <div class="d-flex justify-content-between align-items-center mb-2">
                                        <span class="badge bg-info text-dark rounded-pill">Factura N° <%# Eval("NumeroFactura") %></span>
                                        <small class="text-muted"><%# Convert.ToDateTime(Eval("Fecha")).ToString("dd/MM/yyyy HH:mm") %></small>
                                    </div>
                                    <h4 class="card-title text-success fw-bold mt-3 mb-0">$<%# Convert.ToDecimal(Eval("Total")).ToString("N2") %></h4>
                                </div>
                                <div class="card-footer bg-transparent border-0 pt-0">
                                    <small class="text-muted">Estado: <%# (bool)Eval("estado") ? "<span class='text-success fw-bold'>Activa</span>" : "<span class='text-danger fw-bold'>Inactiva</span>" %></small>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
                <asp:Label ID="lblSinActividad" runat="server" Visible="false" CssClass="text-muted fs-5 ms-3">
                    No tienes operaciones recientes.
                </asp:Label>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlMensajeAnonimo" runat="server" Visible="true" CssClass="text-center mt-5">
            <h4>Bienvenido. Por favor, inicia sesión para acceder a las opciones del sistema.</h4>
        </asp:Panel>

    </main>

</asp:Content> 
