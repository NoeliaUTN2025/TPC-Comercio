USE [tpc_P3]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- SEGURIDAD
-- ============================================================

CREATE TABLE [dbo].[Perfiles] (
    [Id]           [int]         IDENTITY(1,1) NOT NULL,
    [NombrePerfil] [varchar](50) NOT NULL,
    [Estado]       [bit]         NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Usuarios] (
    [Id]         [int]          IDENTITY(1,1) NOT NULL,
    [User]       [varchar](50)  NOT NULL,
    [Contrasena] [varchar](256) NOT NULL,
    [IdPerfil]   [int]          NOT NULL,
    [Estado]     [bit]          NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([User] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Usuarios] WITH CHECK ADD CONSTRAINT [FK_Usuarios_Perfil] FOREIGN KEY([IdPerfil])
REFERENCES [dbo].[Perfiles] ([Id])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Perfil]
GO

-- ============================================================
-- CATALOGO
-- ============================================================

CREATE TABLE [dbo].[Categorias] (
    [Id]          [int]          IDENTITY(1,1) NOT NULL,
    [Descripcion] [varchar](100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Marcas] (
    [Id]          [int]          IDENTITY(1,1) NOT NULL,
    [Descripcion] [varchar](100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Productos] (
    [Id]                 [int]          IDENTITY(1,1) NOT NULL,
    [Codigo]             [varchar](50)  NOT NULL,
    [NombreProducto]     [varchar](150) NOT NULL,
    [Descripcion]        [varchar](500) NULL,
    [StockActual]        [int]          NOT NULL DEFAULT (0),
    [StockMinimo]        [int]          NOT NULL DEFAULT (0),
    [PorcentajeGanancia] [decimal](5,2) NOT NULL DEFAULT (0),
    [IdMarca]            [int]          NOT NULL,
    [IdCategoria]        [int]          NOT NULL,
    [Estado]             [bit]          NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Codigo] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Productos] WITH CHECK ADD CONSTRAINT [FK_Productos_Marca] FOREIGN KEY([IdMarca])
REFERENCES [dbo].[Marcas] ([Id])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Marca]
GO

ALTER TABLE [dbo].[Productos] WITH CHECK ADD CONSTRAINT [FK_Productos_Categoria] FOREIGN KEY([IdCategoria])
REFERENCES [dbo].[Categorias] ([Id])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Categoria]
GO

-- ============================================================
-- CLIENTES
-- ============================================================

CREATE TABLE [dbo].[Clientes] (
    [ID]        [int]          IDENTITY(1,1) NOT NULL,
    [DNI]       [varchar](15)  NOT NULL,
    [Nombre]    [varchar](100) NOT NULL,
    [Apellido]  [varchar](100) NOT NULL,
    [Direccion] [varchar](200) NULL,
    [Telefono]  [varchar](20)  NULL,
    [Email]     [varchar](150) NULL,
    [Estado]    [bit]          NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([ID] ASC),
    UNIQUE NONCLUSTERED ([DNI] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- PROVEEDORES
-- ============================================================

CREATE TABLE [dbo].[Proveedores] (
    [ID]          [int]          IDENTITY(1,1) NOT NULL,
    [RazonSocial] [varchar](150) NOT NULL,
    [Cuit]        [varchar](20)  NOT NULL,
    [Direccion]   [varchar](200) NULL,
    [Telefono]    [varchar](20)  NULL,
    [Email]       [varchar](150) NULL,
    [Estado]      [bit]          NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([ID] ASC),
    UNIQUE NONCLUSTERED ([Cuit] ASC)
) ON [PRIMARY]
GO

-- ============================================================
-- COMPRAS
-- ============================================================

CREATE TABLE [dbo].[Compras] (
    [Id]          [int]           IDENTITY(1,1) NOT NULL,
    [Fecha]       [datetime]      NOT NULL DEFAULT (getdate()),
    [IdProveedor] [int]           NOT NULL,
    [IdUsuario]   [int]           NOT NULL,
    [Total]       [decimal](12,2) NOT NULL DEFAULT (0),
    [Estado]      [bit]           NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Compras] WITH CHECK ADD CONSTRAINT [FK_Compras_Proveedor] FOREIGN KEY([IdProveedor])
REFERENCES [dbo].[Proveedores] ([ID])
GO
ALTER TABLE [dbo].[Compras] CHECK CONSTRAINT [FK_Compras_Proveedor]
GO

ALTER TABLE [dbo].[Compras] WITH CHECK ADD CONSTRAINT [FK_Compras_Usuario] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Compras] CHECK CONSTRAINT [FK_Compras_Usuario]
GO

CREATE TABLE [dbo].[DetalleCompras] (
    [Id]             [int]           IDENTITY(1,1) NOT NULL,
    [IdCompra]       [int]           NOT NULL,
    [IdProducto]     [int]           NOT NULL,
    [Cantidad]       [int]           NOT NULL,
    [PrecioUnitario] [decimal](10,2) NOT NULL,
    [Subtotal]       AS ([Cantidad] * [PrecioUnitario]) PERSISTED,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DetalleCompras] WITH CHECK ADD CONSTRAINT [FK_DetalleCompras_Compra] FOREIGN KEY([IdCompra])
REFERENCES [dbo].[Compras] ([Id])
GO
ALTER TABLE [dbo].[DetalleCompras] CHECK CONSTRAINT [FK_DetalleCompras_Compra]
GO

ALTER TABLE [dbo].[DetalleCompras] WITH CHECK ADD CONSTRAINT [FK_DetalleCompras_Producto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Productos] ([Id])
GO
ALTER TABLE [dbo].[DetalleCompras] CHECK CONSTRAINT [FK_DetalleCompras_Producto]
GO

-- ============================================================
-- VENTAS / FACTURAS
-- ============================================================

CREATE TABLE [dbo].[Facturas] (
    [Id]            [int]           IDENTITY(1,1) NOT NULL,
    [NumeroFactura] [varchar](20)   NOT NULL,
    [Fecha]         [datetime]      NOT NULL DEFAULT (getdate()),
    [IdCliente]     [int]           NOT NULL,
    [IdUsuario]     [int]           NOT NULL,
    [Total]         [decimal](12,2) NOT NULL DEFAULT (0),
    [Estado]        [bit]           NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([NumeroFactura] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CONSTRAINT [FK_Facturas_Cliente] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[Clientes] ([ID])
GO
ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [FK_Facturas_Cliente]
GO

ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CONSTRAINT [FK_Facturas_Usuario] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [FK_Facturas_Usuario]
GO

CREATE TABLE [dbo].[DetalleFacturas] (
    [Id]                 [int]           IDENTITY(1,1) NOT NULL,
    [IdFactura]          [int]           NOT NULL,
    [IdProducto]         [int]           NOT NULL,
    [Cantidad]           [int]           NOT NULL,
    [PrecioCompra]       [decimal](10,2) NOT NULL,
    [PorcentajeGanancia] [decimal](5,2)  NOT NULL,
    [PrecioVenta]        [decimal](10,2) NOT NULL,
    [Subtotal]           AS ([Cantidad] * [PrecioVenta]) PERSISTED,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DetalleFacturas] WITH CHECK ADD CONSTRAINT [FK_DetalleFacturas_Factura] FOREIGN KEY([IdFactura])
REFERENCES [dbo].[Facturas] ([Id])
GO
ALTER TABLE [dbo].[DetalleFacturas] CHECK CONSTRAINT [FK_DetalleFacturas_Factura]
GO

ALTER TABLE [dbo].[DetalleFacturas] WITH CHECK ADD CONSTRAINT [FK_DetalleFacturas_Producto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Productos] ([Id])
GO
ALTER TABLE [dbo].[DetalleFacturas] CHECK CONSTRAINT [FK_DetalleFacturas_Producto]
GO
