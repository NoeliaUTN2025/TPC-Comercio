<%@ Page Title="Reporte de Factura" Language="C#" AutoEventWireup="true" CodeBehind="FacturaReporte.aspx.cs" Inherits="AplicacionWebComercio.FacturaReporte" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Impresión de Factura</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="factura-container">
            <div class="header">
                <h1>TPC Comercio</h1>
                <p>Comprobante de Venta</p>
                <h3 runat="server" id="lblNumeroFactura">Factura N°: -</h3>
                <p runat="server" id="lblFecha">Fecha: -</p>
            </div>

            <div class="info-section">
                <div class="info-box">
                    <h4>Datos del Cliente</h4>
                    <p runat="server" id="lblClienteNombre"><strong>Nombre:</strong> -</p>
                </div>
            </div>

            <asp:GridView ID="dgvDetalles" runat="server" AutoGenerateColumns="False" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="Producto.NombreProducto" HeaderText="Descripción" />
                    <asp:BoundField DataField="Cantidad" HeaderText="Cant." />
                    <asp:BoundField DataField="PrecioVenta" HeaderText="Precio Unit." DataFormatString="{0:C}" />
                    <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:C}" />
                </Columns>
            </asp:GridView>

            <div class="totales">
                <p>Total a Pagar: <strong runat="server" id="lblTotalFinal">$ 0.00</strong></p>
            </div>

            <div class="no-print">
                <button type="button" class="btn" onclick="window.print()">Imprimir Factura</button>
                <a href="Ventas.aspx" class="btn btn-secondary">Volver al Listado</a>
            </div>
        </div>
        
        <asp:Label ID="lblError" runat="server" style="color:red; font-weight:bold; display:block; text-align:center; margin-top:20px;" Visible="false"></asp:Label>
    </form>
</body>
</html>
<style>
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
        margin: 20px; 
        color: #333;
    }

    .factura-container {
        max-width: 800px; 
        margin: 0 auto; 
        border: 1px solid #ddd; 
        padding: 30px; 
        box-shadow: 0 0 10px rgba(0,0,0,0.1);
    }

    .header {
        text-align: center; 
        border-bottom: 2px solid #333; 
        padding-bottom: 20px; 
        margin-bottom: 20px;
    }
    .header h1 {
        margin: 0; 
        font-size: 28px; 
        text-transform: uppercase; 
        letter-spacing: 2px;
    }
    .header p {
        margin: 5px 0; 
        color: #777;
    }

    .info-section {
        display: flex; 
        justify-content: space-between; 
        margin-bottom: 30px;
    }
    .info-box {
        width: 48%;
    }
    .info-box h4 {
        margin-top: 0; 
        border-bottom: 1px solid #eee; 
        padding-bottom: 5px;
    }

    table {
        width: 100%; 
        border-collapse: collapse; 
        margin-bottom: 30px;
    }

    th, td {
        border: 1px solid #ddd; 
        padding: 12px; 
        text-align: left;
    }
    th {
        background-color: #f8f9fa; 
        font-weight: bold;
    }

    .totales{
        text-align: right; 
        font-size: 18px;
    }
    .totales strong {
        font-size: 24px; 
        color: #000;
    }

    .no-print{
        text-align: center; 
        margin-top: 20px;
    }

    .btn {
        padding: 10px 20px; 
        background-color: #007bff; 
        color: white; 
        border: none; 
        cursor: pointer; 
        font-size: 16px; 
        border-radius: 5px; 
        text-decoration: none;
    }
    .btn:hover {
        background-color: #0056b3;
    }
    .btn-secondary {
        background-color: #6c757d; 
        margin-left: 10px;
        padding: 7px 20px 10px;
    }
    
    @media print {
        .no-print {
            display: none;
        }
        .factura-container {
            border: none; 
            box-shadow: none; 
            padding: 0;
        }

    }
</style>