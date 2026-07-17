<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BarraFiltros.ascx.cs" Inherits="AplicacionWebComercio.BarraFiltros" %>

<div class="card shadow-sm mb-4 border-0" style="background: rgba(255,255,255,0.9); backdrop-filter: blur(10px);">
    <div class="card-body">
        <asp:Panel ID="pnlFiltrosContenedor" runat="server" DefaultButton="btnBuscar">
            <h5 class="card-title text-primary mb-3 fw-bold"><i class="fa fa-filter"></i> Búsqueda Rápida</h5>
            <div class="row g-3">
                <div class="col-md-4">
                    <asp:Label ID="lblTexto" runat="server" Text="Buscar:" CssClass="form-label text-muted small fw-bold"></asp:Label>
                    <asp:TextBox ID="txtTexto" runat="server" CssClass="form-control" placeholder="Escriba el nombre..."></asp:TextBox>
                </div>
                
                <asp:Panel ID="pnlCategoria" runat="server" CssClass="col-md-3">
                    <asp:Label ID="lblCategoria" runat="server" Text="Categoría:" CssClass="form-label text-muted small fw-bold"></asp:Label>
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </asp:Panel>
                
                <asp:Panel ID="pnlMarca" runat="server" CssClass="col-md-3">
                    <asp:Label ID="lblMarca" runat="server" Text="Marca:" CssClass="form-label text-muted small fw-bold"></asp:Label>
                    <asp:DropDownList ID="ddlMarca" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </asp:Panel>

                <asp:Panel ID="pnlFechas" runat="server" CssClass="col-md-4" Visible="false">
                    <div class="row">
                        <div class="col-6">
                            <asp:Label ID="lblFechaDesde" runat="server" Text="Desde:" CssClass="form-label text-muted small fw-bold"></asp:Label>
                            <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-6">
                            <asp:Label ID="lblFechaHasta" runat="server" Text="Hasta:" CssClass="form-label text-muted small fw-bold"></asp:Label>
                            <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                </asp:Panel>

                <div class="col-md-12 d-flex align-items-end justify-content-end mt-4">
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar Filtros" CssClass="btn btn-outline-secondary me-2" OnClick="btnLimpiar_Click" />
                    <asp:Button ID="btnBuscar" runat="server" Text="Aplicar Filtro" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>
</div>
