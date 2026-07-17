<%@ Page Title="Reportes Dinámicos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportesDinamicos.aspx.cs" Inherits="AplicacionWebComercio.ReportesDinamicos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Generador de Reportes</h2>
        <hr />

        <div class="card shadow-sm mb-4">
            <div class="card-header bg-light d-flex justify-content-between align-items-center">
                <h5 class="mb-0">Plantillas Guardadas</h5>
            </div>
            <div class="card-body">
                <div class="row g-3 align-items-end">
                    <div class="col-md-6">
                        <label class="form-label fw-bold">Cargar Plantilla Favorita</label>
                        <div class="input-group">
                            <asp:DropDownList ID="ddlPlantillas" runat="server" CssClass="form-select" DataTextField="DescripcionCombo" DataValueField="Id">
                            </asp:DropDownList>
                            <asp:Button ID="btnCargarPlantilla" runat="server" Text="Cargar" CssClass="btn btn-secondary" OnClick="btnCargarPlantilla_Click" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-bold">Guardar Filtros Actuales</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtNombrePlantilla" runat="server" CssClass="form-control" placeholder="Nombre de la nueva plantilla..."></asp:TextBox>
                            <asp:Button ID="btnGuardarPlantilla" runat="server" Text="Guardar Plantilla" CssClass="btn btn-outline-primary" OnClick="btnGuardarPlantilla_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="card shadow-sm mb-4">
            <div class="card-header bg-light">
                <h5 class="mb-0">Configuración del Reporte</h5>
            </div>
            <div class="card-body">
                <div class="row g-3 align-items-end">
                    <div class="col-md-3">
                        <label class="form-label fw-bold">Tipo de Reporte</label>
                        <asp:DropDownList ID="ddlTipoReporte" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Seleccione un reporte..." Value="" />
                            <asp:ListItem Text="Historial de Ventas" Value="Ventas" />
                            <asp:ListItem Text="Historial de Compras" Value="Compras" />
                            <asp:ListItem Text="Producto más vendido (Top 10)" Value="ProductoMasVendido" />
                            <asp:ListItem Text="Lotes más costosos (Top 10)" Value="LoteMasCostoso" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label fw-bold">Fecha Desde</label>
                        <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label fw-bold">Fecha Hasta</label>
                        <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="col-md-3">
                        <asp:Button ID="btnGenerar" runat="server" Text="Generar Reporte" CssClass="btn btn-primary w-100 mb-2" OnClick="btnGenerar_Click" />
                        <asp:Button ID="btnExportar" runat="server" Text="Exportar a CSV" CssClass="btn btn-success w-100" OnClick="btnExportar_Click" Visible="false" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger d-block mb-3"></asp:Label>

        <div class="table-responsive">
            <asp:GridView ID="dgvReporte" runat="server" CssClass="table table-striped table-hover table-bordered" 
                AutoGenerateColumns="true" EmptyDataText="No hay datos para mostrar con los filtros seleccionados.">
                <HeaderStyle CssClass="table-dark" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
