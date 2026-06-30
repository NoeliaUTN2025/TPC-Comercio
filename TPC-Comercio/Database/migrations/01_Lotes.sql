-- Script de migracion: Lotes
-- Ejecutar contra tpc_P3 en orden
USE [tpc_P3]
GO

-- 1. Tabla Lotes
CREATE TABLE [dbo].[Lotes] (
    [Id]              [int]           IDENTITY(1,1) NOT NULL,
    [IdProducto]      [int]           NOT NULL,
    [IdDetalleCompra] [int]           NOT NULL,
    [CantidadTotal]   [int]           NOT NULL,
    [CantidadDisp]    [int]           NOT NULL,
    [PrecioCompra]    [decimal](10,2) NOT NULL,
    [FechaIngreso]    [datetime]      NOT NULL DEFAULT (getdate()),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Lotes_Producto]      FOREIGN KEY([IdProducto])      REFERENCES [dbo].[Productos] ([Id]),
    CONSTRAINT [FK_Lotes_DetalleCompra] FOREIGN KEY([IdDetalleCompra]) REFERENCES [dbo].[DetalleCompras] ([Id])
) ON [PRIMARY]
GO

-- 2. Columna IdLote en DetalleFacturas
ALTER TABLE [dbo].[DetalleFacturas] ADD [IdLote] [int] NULL
GO

ALTER TABLE [dbo].[DetalleFacturas] ADD CONSTRAINT [FK_DetalleFacturas_Lote]
    FOREIGN KEY([IdLote]) REFERENCES [dbo].[Lotes] ([Id])
GO

-- 3. SPs de Lotes
CREATE OR ALTER PROCEDURE [dbo].[SP_Lotes_Crear]
    @IdProducto      int,
    @IdDetalleCompra int,
    @Cantidad        int,
    @PrecioCompra    decimal(10,2),
    @NewId           int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[Lotes] (IdProducto, IdDetalleCompra, CantidadTotal, CantidadDisp, PrecioCompra)
    VALUES (@IdProducto, @IdDetalleCompra, @Cantidad, @Cantidad, @PrecioCompra)
    SET @NewId = SCOPE_IDENTITY()
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Lotes_ListarPorProducto]
    @IdProducto int
AS
BEGIN
    SELECT
        l.Id,
        l.IdProducto,
        l.IdDetalleCompra,
        l.CantidadTotal,
        l.CantidadDisp,
        l.PrecioCompra,
        l.FechaIngreso,
        prov.RazonSocial AS Proveedor
    FROM [dbo].[Lotes] l
    INNER JOIN [dbo].[DetalleCompras] dc   ON l.IdDetalleCompra = dc.Id
    INNER JOIN [dbo].[Compras]        c    ON dc.IdCompra       = c.Id
    INNER JOIN [dbo].[Proveedores]    prov ON c.IdProveedor     = prov.ID
    WHERE l.IdProducto = @IdProducto AND l.CantidadDisp > 0
    ORDER BY l.FechaIngreso ASC
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Lotes_DescontarStock]
    @Id       int,
    @Cantidad int
AS
BEGIN
    UPDATE [dbo].[Lotes]
    SET CantidadDisp = CantidadDisp - @Cantidad
    WHERE Id = @Id AND CantidadDisp >= @Cantidad

    IF @@ROWCOUNT = 0
        RAISERROR('Stock insuficiente o lote no encontrado para el Id indicado.', 16, 1)
END
GO
