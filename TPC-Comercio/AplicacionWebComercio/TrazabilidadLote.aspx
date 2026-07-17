<%@ Page Title="Trazabilidad de Lotes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrazabilidadLote.aspx.cs" Inherits="AplicacionWebComercio.TrazabilidadLote" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">

        <h2>Trazabilidad de Lotes</h2>
        <p class="text-muted">Seguimiento completo del ciclo de vida de un producto: compra → lote → venta → stock.</p>

        <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert alert-danger d-block w-100 mb-3" />

        <div class="card mb-4">
            <div class="card-body">
                <div class="row align-items-end g-3">
                    <div class="col-md-8">
                        <label class="form-label fw-bold">Seleccioná un producto</label>
                        <asp:DropDownList runat="server" ID="ddlProducto" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlProducto_SelectedIndexChanged" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel runat="server" ID="pnlResultados" Visible="false">
        
            <asp:Panel ID="pnlProducto" runat="server" CssClass="card mb-4">
                <div class="card-header">
                    <strong>Producto seleccionado</strong>
                    </div>
                <div class="card-body">
                      
                    <h3 class="text-primary mb-3">
                        <asp:Literal runat="server" ID="litProducto" />
                    </h3>

                      <div class="row">
                      <div class="col-md-6">
                          <p><strong>Código:</strong> <asp:Literal runat="server" ID="litCodigo" /></p>
                          <p><strong>Marca:</strong> <asp:Literal runat="server" ID="litMarca" /></p>
                          <p><strong>Categoría:</strong> <asp:Literal runat="server" ID="litCategoria" /></p>
                          <p>
                              <strong>Ganancia:</strong>
                              <span class="label label-success"> <asp:Literal runat="server" ID="litGanancia" />
                              </span>
                          </p>
                        </div>
                          <div class="col-md-4 text-center justify-content-center">
                              <asp:Image ID="imgProducto" runat="server" CssClass="img-thumbnail" Width="220px" ImageUrl="~/Images/sin-image.png" Style="margin-top:-50px;" />
                          </div>
                      </div>
                    <hr />
                    <p>
                    <p><strong>Descripción:</strong> <asp:Literal runat="server" ID="litDescripcion" /></p>
                    </p>
             </div>
            </asp:Panel>
            <div class="alert alert-primary mb-4">
                <strong>Stock actual disponible:</strong>
                <asp:Literal runat="server" ID="litStockActual" /> unidades
            </div>

            <h4>Lotes registrados</h4>
            <asp:GridView runat="server" ID="dgvLotes" CssClass="table table-bordered table-hover"
                AutoGenerateColumns="false" EmptyDataText="Sin lotes para este producto.">
                <Columns>
                    <asp:BoundField DataField="Id"                HeaderText="Lote #" />
                    <asp:BoundField DataField="FechaIngreso"      HeaderText="Ingreso"      DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                    <asp:BoundField DataField="Proveedor"         HeaderText="Proveedor" />
                    <asp:BoundField DataField="CantidadTotal"     HeaderText="Comprado" />
                    <asp:BoundField DataField="CantidadVendida"   HeaderText="Vendido" />
                    <asp:BoundField DataField="CantidadDisp"      HeaderText="Disponible"   ItemStyle-CssClass="fw-bold text-success" />
                    <asp:BoundField DataField="PrecioCompra"      HeaderText="P. Compra"    DataFormatString="{0:C}" HtmlEncode="false" />
                    <asp:BoundField DataField="PrecioTotal"      HeaderText="Total Compra"    DataFormatString="{0:C}" HtmlEncode="false" />
                    <asp:BoundField DataField="PrecioVenta"       HeaderText="P. Venta"     DataFormatString="{0:C}" HtmlEncode="false" />
                    <asp:BoundField DataField="GananciaRealizada" HeaderText="Ganancia real" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="text-success" />
                </Columns>
            </asp:GridView> 

            <h4 class="mt-4">Ventas realizadas desde estos lotes</h4>
            <asp:GridView runat="server" ID="dgvVentas" CssClass="table table-bordered table-hover"
                AutoGenerateColumns="false" EmptyDataText="Sin ventas registradas aún.">
                <Columns>
                    <asp:BoundField DataField="IdLote"        HeaderText="Lote #" />
                    <asp:BoundField DataField="NumeroFactura" HeaderText="Factura N°" />
                    <asp:BoundField DataField="FechaVenta"    HeaderText="Fecha venta"  DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                    <asp:BoundField DataField="Cliente"       HeaderText="Cliente" />
                    <asp:BoundField DataField="Cantidad"      HeaderText="Cant." />
                    <asp:BoundField DataField="PrecioVenta"   HeaderText="P. Venta"     DataFormatString="{0:C}" HtmlEncode="false" />
                    <asp:BoundField DataField="Subtotal"      HeaderText="Subtotal"     DataFormatString="{0:C}" HtmlEncode="false" />
                </Columns>
            </asp:GridView>

        </asp:Panel>
    </div>
</asp:Content>
