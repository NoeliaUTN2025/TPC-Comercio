<%@ Page Title="Factura de Venta" Language="C#" AutoEventWireup="true" CodeBehind="FacturaReporte.aspx.cs" Inherits="AplicacionWebComercio.FacturaReporte" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Factura de Venta</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; color: #333; }
        .factura-container { max-width: 800px; margin: 0 auto; border: 1px solid #ddd; padding: 30px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }
        .header { text-align: center; border-bottom: 2px solid #333; padding-bottom: 20px; margin-bottom: 20px; }
        .header h1 { margin: 0; font-size: 28px; text-transform: uppercase; letter-spacing: 2px; }
        .header .subtitulo { margin: 4px 0; color: #777; }
        .header .numero { font-size: 16px; font-weight: bold; margin-top: 8px; }
        .info-section { display: flex; justify-content: space-between; margin-bottom: 30px; }
        .info-box { width: 48%; }
        .info-box h4 { margin-top: 0; border-bottom: 1px solid #eee; padding-bottom: 5px; }
        .info-box p { margin: 4px 0; font-size: 14px; }
        table { width: 100%; border-collapse: collapse; margin-bottom: 30px; }
        th, td { border: 1px solid #ddd; padding: 10px 12px; text-align: left; }
        th { background-color: #f8f9fa; font-weight: bold; }
        td.num { text-align: right; }
        .totales { text-align: right; font-size: 18px; border-top: 2px solid #333; padding-top: 10px; }
        .totales strong { font-size: 24px; }
        .no-print { text-align: center; margin-top: 24px; }
        .btn { padding: 10px 20px; background-color: #007bff; color: white; border: none; cursor: pointer; font-size: 15px; border-radius: 5px; text-decoration: none; display: inline-block; }
        .btn:hover { background-color: #0056b3; }
        .btn-secondary { background-color: #6c757d; margin-left: 10px; }
        @media print {
            .no-print { display: none; }
            .factura-container { border: none; box-shadow: none; padding: 0; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="factura-container">

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
                <button type="button" class="btn" onclick="window.print()">Imprimir</button>
                <a href="Ventas.aspx" class="btn btn-secondary">Volver al listado</a>
            </div>

            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"
                style="display:block;margin-top:20px;text-align:center;font-weight:bold;" />
        </div>
    </form>
</body>
</html>
