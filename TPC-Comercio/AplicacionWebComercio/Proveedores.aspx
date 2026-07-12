<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Proveedores.aspx.cs" Inherits="AplicacionWebComercio.Proveedores" %>
<%@ Register Src="~/BarraFiltros.ascx" TagPrefix="uc1" TagName="BarraFiltros" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Gestión de Proveedores</h2>

    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo Proveedor" OnClick="btnNuevo_Click" CssClass="btn btn-success mb-3" />
    
    <uc1:BarraFiltros runat="server" ID="ctrlFiltros" OnFiltrar="ctrlFiltros_Filtrar" MostrarCategoria="false" MostrarMarca="false" MostrarFechas="false" />

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card p-3 mb-3">
        <h4>Datos del Proveedor</h4>
        <asp:HiddenField ID="hfId" runat="server" Value="0" />

        <div class="mb-2">
            <label>Razón Social</label>
            <asp:TextBox ID="txtRazonSocial" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>CUIT</label>
            <asp:TextBox ID="txtCuit" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Dirección</label>
            <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Teléfono</label>
            <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-2">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary me-2" />
        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="btn btn-secondary" CausesValidation="false" />
    </asp:Panel>

    <asp:GridView ID="dgvProveedores" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
        DataKeyNames="ID" OnRowCommand="dgvProveedores_RowCommand" OnRowDeleting="dgvProveedores_RowDeleting">
        <Columns>
            <asp:BoundField DataField="RazonSocial" HeaderText="Razón Social" />
            <asp:BoundField DataField="Cuit" HeaderText="CUIT" />
            <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEditar" runat="server" Text="Editar"
                        CommandName="Editar" CommandArgument='<%# Eval("ID") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEliminar" runat="server" Text="Eliminar" CommandName="Delete"
                        OnClientClick="return confirm('¿Confirma la baja del proveedor?');" CssClass="text-danger" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>
