<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Categorias.aspx.cs" Inherits="AplicacionWebComercio.Categorias" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Gestión de Categorías</h2>

    <asp:Button ID="btnNuevo" runat="server" Text="Nueva Categoría" OnClick="btnNuevo_Click" CssClass="btn btn-success mb-3" />

    <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card p-3 mb-3">
        <h4>Datos de la Categoría</h4>
        <asp:HiddenField ID="hfId" runat="server" Value="0" />

        <div class="mb-2">
            <label>Descripción</label>
            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary me-2" />
        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="btn btn-secondary" CausesValidation="false" />
    </asp:Panel>

    <asp:GridView ID="dgvCategorias" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
        DataKeyNames="Id" OnRowCommand="dgvCategorias_RowCommand" OnRowDeleting="dgvCategorias_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEditar" runat="server" Text="Editar"
                        CommandName="Editar" CommandArgument='<%# Eval("Id") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEliminar" runat="server" Text="Eliminar" CommandName="Delete"
                        OnClientClick="return confirm('¿Confirma la eliminación de la categoría?');" CssClass="text-danger" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block mt-2" Visible="false" />

</asp:Content>
