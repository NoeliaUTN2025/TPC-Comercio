<%@ Page Title="Factura de Venta" Language="C#" AutoEventWireup="true" CodeBehind="FacturaReporte.aspx.cs" Inherits="AplicacionWebComercio.FacturaReporte" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Factura de Venta</title>
    <link href="Content/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="comprobante-container">

            <div class="header">
                <h1>TPC Comercio</h1>
                <p class="subtitulo">Comprobante de Venta</p>
                <p class="numero" runat="server" id="lblNumeroFactura">Factura N°: -</p>
                <p class="subtitulo" runat="server" id="lblFecha">Fecha: -</p>
            </div>

            <div class="info-section">
                <div class="info-box">
                    <h4>Cliente</h4>
                    <p runat="server" id="lblClienteNombre">-</p>
                </div>
                <div class="info-box">
                    <h4>Datos de la factura</h4>
                    <asp:Literal runat="server" ID="litDatosFactura" />
                </div>
            </div>

            <asp:GridView ID="dgvDetalles" runat="server" AutoGenerateColumns="False" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="Producto.NombreProducto" HeaderText="Descripción" />
                    <asp:BoundField DataField="Cantidad"    HeaderText="Cant."       ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="PrecioVenta" HeaderText="P. Unitario" DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="Subtotal"    HeaderText="Subtotal"    DataFormatString="{0:C}" HtmlEncode="false" ItemStyle-CssClass="num" />
                </Columns>
            </asp:GridView>

            <div class="totales">
                Total: <strong runat="server" id="lblTotalFinal">$ 0,00</strong>
            </div>

            <div class="no-print">
                <button type="button" class="btn-comprobante" onclick="window.print()">Imprimir</button>
                <a href="Ventas.aspx" class="btn-comprobante btn-comprobante-secondary">Volver al listado</a>
            </div>

            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"
                style="display:block;margin-top:20px;text-align:center;font-weight:bold;" />
        </div>
    </form>
</body>
</html>
