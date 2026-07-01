<%@ Page Title="Mis Propuestas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MisCompras.aspx.cs" Inherits="AplicacionWebComercio.MisCompras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container mt-4">

        <div class="row">
            <%-- Mis compras (lo que el negocio le compró al proveedor) --%>
            <div class="col-md-7">
                <h4>Lotes aceptados</h4>
                <p class="text-muted small">Propuestas que el negocio aprobó y pagó</p>

                <asp:GridView runat="server" ID="dgvMisCompras" CssClass="table table-bordered table-hover"
                    AutoGenerateColumns="false" EmptyDataText="Todavía no hay compras registradas.">
                    <Columns>
                        <asp:BoundField DataField="Id"     HeaderText="#"      ItemStyle-Width="40" />
                        <asp:BoundField DataField="Fecha"  HeaderText="Fecha"  DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                        <asp:BoundField DataField="Total"  HeaderText="Total"  DataFormatString="{0:C}" HtmlEncode="false" />
                        <asp:BoundField DataField="CantidadTotal" HeaderText="Unidades" />
                    </Columns>
                </asp:GridView>
            </div>

            <%-- Proponer nuevo lote --%>
            <div class="col-md-5">
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">Proponer un lote</h5>
                    </div>
                    <div class="card-body">

                        <asp:Label runat="server" ID="lblMensaje" CssClass="alert d-none w-100 mb-3" />

                        <div class="mb-3">
                            <label class="form-label">Producto</label>
                            <asp:DropDownList runat="server" ID="ddlProducto" CssClass="form-select" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Cantidad</label>
                            <asp:TextBox runat="server" ID="txtCantidad" CssClass="form-control" placeholder="Ej: 50" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Precio unitario ($)</label>
                            <asp:TextBox runat="server" ID="txtPrecio" CssClass="form-control" placeholder="Ej: 1500,00" />
                        </div>
                        <asp:Button runat="server" ID="btnProponer" Text="Enviar propuesta" CssClass="btn btn-success w-100" OnClick="btnProponer_Click" />

                    </div>
                </div>

                <%-- Mis propuestas --%>
                <h5 class="mt-4">Mis propuestas</h5>
                <asp:GridView runat="server" ID="dgvPropuestas" CssClass="table table-sm table-bordered"
                    AutoGenerateColumns="false" EmptyDataText="Sin propuestas aún.">
                    <Columns>
                        <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                        <asp:BoundField DataField="Cantidad"        HeaderText="Cant."   />
                        <asp:BoundField DataField="PrecioUnitario"  HeaderText="Precio"  DataFormatString="{0:C}" HtmlEncode="false" />
                        <asp:BoundField DataField="Estado"          HeaderText="Estado"  />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>

</asp:Content>
