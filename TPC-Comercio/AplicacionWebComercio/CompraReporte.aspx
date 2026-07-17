<%@ Page Title="Orden de Compra" Language="C#" AutoEventWireup="true" CodeBehind="CompraReporte.aspx.cs" Inherits="AplicacionWebComercio.CompraReporte" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Orden de Compra</title>
    <link href="Content/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="comprobante-container">

            <div class="header">
                <h1>TPC Comercio</h1>
                <p class="subtitulo">Orden de Compra</p>
                <p class="numero" runat="server" id="lblNumeroCompra">N° -</p>
                <p class="subtitulo" runat="server" id="lblFecha">Fecha: -</p>
            </div>

            <div class="info-section">
                <div class="info-box">
                    <h4>Proveedor</h4>
                    <asp:Literal runat="server" ID="litProveedor" />
                </div>
                <div class="info-box">
                    <h4>Datos de la compra</h4>
                    <asp:Literal runat="server" ID="litDatosCompra" />
                </div>
            </div>

            <asp:GridView ID="dgvDetalles" runat="server" AutoGenerateColumns="false" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="NombreProducto" HeaderText="Producto" />
                    <asp:BoundField DataField="Cantidad"       HeaderText="Cant."    ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="PrecioUnitario" HeaderText="P. Unitario" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="Subtotal"       HeaderText="Subtotal"    DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="num" />
                </Columns>
            </asp:GridView>

            <div class="totales">
                Total: <strong runat="server" id="lblTotal">$ 0,00</strong>
            </div>

            <div class="no-print">
                <button type="button" class="btn-comprobante" onclick="window.print()">Imprimir</button>
                <a runat="server" id="lnkVolver" href="Compras.aspx" class="btn-comprobante btn-comprobante-secondary">Volver al listado</a>
            </div>

            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false" style="display:block;margin-top:20px;text-align:center;" />
        </div>
    </form>
</body>
</html>
