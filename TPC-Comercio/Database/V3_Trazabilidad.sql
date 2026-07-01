USE tpc_P3
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_Trazabilidad_Lotes')
    DROP PROCEDURE SP_Trazabilidad_Lotes
GO

CREATE PROCEDURE SP_Trazabilidad_Lotes
    @IdProducto INT
AS
BEGIN
    SELECT
        l.Id,
        l.FechaIngreso,
        prov.RazonSocial AS Proveedor,
        l.CantidadTotal,
        l.CantidadDisp,
        l.PrecioCompra
    FROM Lotes l
    INNER JOIN DetalleCompras dc  ON l.IdDetalleCompra = dc.Id
    INNER JOIN Compras c          ON dc.IdCompra       = c.Id
    INNER JOIN Proveedores prov   ON c.IdProveedor     = prov.ID
    WHERE l.IdProducto = @IdProducto
    ORDER BY l.FechaIngreso ASC
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_Trazabilidad_Ventas')
    DROP PROCEDURE SP_Trazabilidad_Ventas
GO

CREATE PROCEDURE SP_Trazabilidad_Ventas
    @IdProducto INT
AS
BEGIN
    SELECT
        df.IdLote,
        f.NumeroFactura,
        f.Fecha                              AS FechaVenta,
        cli.Nombre + ' ' + cli.Apellido      AS Cliente,
        df.Cantidad,
        df.PrecioVenta,
        df.Subtotal
    FROM DetalleFacturas df
    INNER JOIN Facturas f   ON df.IdFactura  = f.Id
    INNER JOIN Clientes cli ON f.IdCliente   = cli.ID
    WHERE df.IdProducto = @IdProducto
      AND df.IdLote IS NOT NULL
    ORDER BY df.IdLote, f.Fecha
END
GO
