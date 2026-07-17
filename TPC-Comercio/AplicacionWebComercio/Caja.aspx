<%@ Page Title="Caja" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Caja.aspx.cs" Inherits="AplicacionWebComercio.Caja" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">Caja</h2>
    </div>

    <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-none w-100 mb-3" />

    <asp:Panel ID="pnlAbrirCaja" runat="server" CssClass="card mb-4">
        <div class="card-header">
            <h5 class="mb-0">Abrir caja</h5>
        </div>
        <div class="card-body">
            <div class="row g-2 align-items-end">
                <div class="col-md-3">
                    <label class="form-label">Monto inicial ($)</label>
                    <asp:TextBox ID="txtMontoApertura" runat="server" CssClass="form-control" ValidationGroup="AbrirCaja" placeholder="0,00" />
                    <asp:RequiredFieldValidator ID="rfvMontoApertura" runat="server" ControlToValidate="txtMontoApertura" ErrorMessage="*" CssClass="text-danger" ValidationGroup="AbrirCaja" Display="Dynamic" />
                    <asp:RangeValidator ID="rvMontoApertura" runat="server" ControlToValidate="txtMontoApertura" MinimumValue="0" MaximumValue="99999999" Type="Double" ErrorMessage="Inválido" CssClass="text-danger" ValidationGroup="AbrirCaja" Display="Dynamic" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnAbrirCaja" runat="server" Text="Abrir caja" CssClass="btn btn-primary" OnClick="btnAbrirCaja_Click" ValidationGroup="AbrirCaja" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlCajaAbierta" runat="server" CssClass="card mb-4" Visible="false">
        <div class="card-header">
            <h5 class="mb-0">Caja abierta</h5>
        </div>
        <div class="card-body">
            <p class="mb-1"><strong>Apertura:</strong> <asp:Literal ID="litFechaApertura" runat="server" /> — <asp:Literal ID="litMontoApertura" runat="server" /></p>
            <p class="mb-3"><strong>Efectivo cobrado hasta el momento:</strong> <asp:Literal ID="litEfectivoAcumulado" runat="server" /></p>
            <hr />
            <div class="row g-2 align-items-end">
                <div class="col-md-3">
                    <label class="form-label">Monto contado al cierre ($)</label>
                    <asp:TextBox ID="txtMontoCierre" runat="server" CssClass="form-control" ValidationGroup="CerrarCaja" placeholder="0,00" />
                    <asp:RequiredFieldValidator ID="rfvMontoCierre" runat="server" ControlToValidate="txtMontoCierre" ErrorMessage="*" CssClass="text-danger" ValidationGroup="CerrarCaja" Display="Dynamic" />
                    <asp:RangeValidator ID="rvMontoCierre" runat="server" ControlToValidate="txtMontoCierre" MinimumValue="0" MaximumValue="99999999" Type="Double" ErrorMessage="Inválido" CssClass="text-danger" ValidationGroup="CerrarCaja" Display="Dynamic" />
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnCerrarCaja" runat="server" Text="Cerrar caja" CssClass="btn btn-primary" OnClick="btnCerrarCaja_Click" ValidationGroup="CerrarCaja" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <div class="card">
        <div class="card-header">
            <h5 class="mb-0">Histórico de cajas</h5>
        </div>
        <div class="card-body p-0">
            <asp:GridView ID="dgvHistorico" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-sm mb-0" EmptyDataText="Sin cajas registradas.">
                <Columns>
                    <asp:BoundField DataField="FechaApertura"        HeaderText="Apertura"       DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="UsuarioApertura"      HeaderText="Abrió" />
                    <asp:BoundField DataField="MontoApertura"        HeaderText="Monto apertura"  DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    <asp:BoundField DataField="FechaCierre"          HeaderText="Cierre"         DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="UsuarioCierre"        HeaderText="Cerró" />
                    <asp:BoundField DataField="MontoCierreDeclarado" HeaderText="Declarado"      DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    <asp:BoundField DataField="MontoCierreCalculado" HeaderText="Calculado"      DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                    <asp:BoundField DataField="Diferencia"           HeaderText="Diferencia"     DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-end" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
