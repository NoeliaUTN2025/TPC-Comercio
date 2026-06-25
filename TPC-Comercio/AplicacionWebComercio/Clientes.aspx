<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Clientes.aspx.cs" Inherits="AplicacionWebComercio.Clientes" %>
    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

        <h2>Gestión de Clientes</h2>

        <asp:Button ID="btnNuevo" runat="server" Text="Nuevo Cliente" OnClick="btnNuevo_Click"
            CssClass="btn btn-success mb-3" />

        <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card p-3 mb-3">
            <h4>Datos del Cliente</h4>
            <asp:HiddenField ID="hfId" runat="server" Value="0" />

            <div class="row">
                <div class="col-md-4 mb-2">
                    <label>DNI <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtDNI" runat="server" TextMode="Number" CssClass="form-control" MaxLength="15" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDNI"
                        ErrorMessage="El DNI es obligatorio." CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-4 mb-2">
                    <label>Nombre <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" MaxLength="100" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNombre"
                        ErrorMessage="El nombre es obligatorio." CssClass="text-danger small" Display="Dynamic" />
                </div>
                <div class="col-md-4 mb-2">
                    <label>Apellido <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" MaxLength="100" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtApellido"
                        ErrorMessage="El apellido es obligatorio." CssClass="text-danger small" Display="Dynamic" />
                </div>
            </div>

            <div class="row">
                <div class="col-md-4 mb-2">
                    <label>Dirección</label>
                    <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" MaxLength="200" />
                </div>
                <div class="col-md-4 mb-2">
                    <label>Teléfono</label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" MaxLength="20" />
                </div>
                <div class="col-md-4 mb-2">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="150"
                        TextMode="Email" />
                </div>
            </div>

            <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click"
                CssClass="btn btn-primary me-2" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click"
                CssClass="btn btn-secondary" CausesValidation="false" />
        </asp:Panel>

        <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block mb-2" Visible="false" />

        <asp:GridView ID="dgvClientes" runat="server" AutoGenerateColumns="false"
            CssClass="table table-striped table-hover" DataKeyNames="ID" OnRowCommand="dgvClientes_RowCommand"
            OnRowDeleting="dgvClientes_RowDeleting">
            <Columns>
                <asp:BoundField DataField="DNI" HeaderText="DNI" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEditar" runat="server" Text="Editar" CommandName="Editar"
                            CommandArgument='<%# Eval("ID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEliminar" runat="server" Text="Eliminar" CommandName="Delete"
                            OnClientClick="return confirm('¿Confirma la baja del cliente?');" CssClass="text-danger" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </asp:Content>