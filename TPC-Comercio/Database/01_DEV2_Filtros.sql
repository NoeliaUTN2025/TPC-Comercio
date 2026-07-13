USE tpc_P3
GO

-- ============================================================
-- DEV 2: FILTRADO DINAMICO
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Productos_Filtrar]
    @Texto VARCHAR(100) = NULL,
    @IdCategoria INT = NULL,
    @IdMarca INT = NULL
AS
BEGIN
    SELECT
        p.Id,
        p.Codigo,
        p.NombreProducto,
        p.Descripcion,
        p.StockActual,
        p.StockMinimo,
        p.Precio,
        p.PorcentajeGanancia,
        p.IdMarca,
        m.Descripcion AS Marca,
        p.IdCategoria,
        c.Descripcion AS Categoria
    FROM [dbo].[Productos] p
        INNER JOIN [dbo].[Marcas]     m ON p.IdMarca     = m.Id
        INNER JOIN [dbo].[Categorias] c ON p.IdCategoria = c.Id
    WHERE p.Estado = 1
        AND (@Texto IS NULL OR p.NombreProducto LIKE '%' + @Texto + '%' OR p.Codigo LIKE '%' + @Texto + '%')
        AND (@IdCategoria IS NULL OR p.IdCategoria = @IdCategoria)
        AND (@IdMarca IS NULL OR p.IdMarca = @IdMarca)
END
GO

CREATE PROCEDURE [dbo].[SP_Proveedores_Filtrar]
    @Texto VARCHAR(100) = NULL
AS
BEGIN
    SELECT ID, RazonSocial, Cuit, Direccion, Telefono, Email
    FROM Proveedores
    WHERE Estado = 1
        AND (@Texto IS NULL OR RazonSocial LIKE '%' + @Texto + '%' OR Cuit LIKE '%' + @Texto + '%' OR Email LIKE '%' + @Texto + '%')
END
GO

CREATE PROCEDURE [dbo].[SP_Compras_Filtrar]
    @Texto VARCHAR(100) = NULL,
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SELECT c.Id, c.Fecha, c.IdProveedor, p.RazonSocial AS Proveedor, c.IdUsuario, c.Total, c.Estado,
        ISNULL((SELECT SUM(Cantidad)
        FROM DetalleCompras dc
        WHERE dc.IdCompra = c.Id), 0) AS CantidadTotal
    FROM Compras c
        INNER JOIN Proveedores p ON c.IdProveedor = p.ID
    WHERE c.Estado = 1
        AND (@Texto IS NULL OR p.RazonSocial LIKE '%' + @Texto + '%')
        AND (@FechaDesde IS NULL OR CAST(c.Fecha AS DATE) >= @FechaDesde)
        AND (@FechaHasta IS NULL OR CAST(c.Fecha AS DATE) <= @FechaHasta)
    ORDER BY c.Fecha DESC
END
GO

CREATE PROCEDURE [dbo].[SP_Ventas_Filtrar]
    @IdCliente INT = NULL,
    @Texto VARCHAR(100) = NULL,
    @FechaDesde DATE = NULL,
    @FechaHasta DATE = NULL
AS
BEGIN
    SELECT F.Id, F.NumeroFactura, F.Fecha, F.Total, F.Estado, C.ID as IdCliente, C.Nombre, C.Apellido
    FROM Facturas F
        INNER JOIN Clientes C ON F.IdCliente = C.ID
    WHERE (@IdCliente IS NULL OR F.IdCliente = @IdCliente)
        AND (@Texto IS NULL OR C.Nombre LIKE '%' + @Texto + '%' OR C.Apellido LIKE '%' + @Texto + '%' OR F.NumeroFactura LIKE '%' + @Texto + '%')
        AND (@FechaDesde IS NULL OR CAST(F.Fecha AS DATE) >= @FechaDesde)
        AND (@FechaHasta IS NULL OR CAST(F.Fecha AS DATE) <= @FechaHasta)
    ORDER BY F.Fecha DESC
END
GO
