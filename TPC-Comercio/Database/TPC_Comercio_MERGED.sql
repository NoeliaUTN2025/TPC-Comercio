-- ============================================================
-- TPC Comercio - Script de Creación Global
-- Base de datos: tpc_P3
-- Merge de: CREATE_JULIAN.sql + CREATE NOELIA.sql + CREATE_Joaquin.sql
-- ============================================================

USE [master]
GO

IF DB_ID('tpc_P3') IS NULL
    CREATE DATABASE [tpc_P3]
GO

USE [tpc_P3]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- AUTENTICACIÓN: Perfiles y Usuarios
-- ============================================================

CREATE TABLE [dbo].[Perfiles]
(
    [Id]           [int]          IDENTITY(1,1) NOT NULL,
    [NombrePerfil] [varchar](100) NOT NULL,
    [Estado]       [bit]          NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Usuarios]
(
    [Id]        [int]          IDENTITY(1,1) NOT NULL,
    [User]      [varchar](50)  NOT NULL,
    [Contrasena][varchar](256) NOT NULL,
    [IdPerfil]  [int]          NOT NULL,
    [Estado]    [bit]          NOT NULL DEFAULT (1),
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
-- CATÁLOGO: Categorias, Marcas, Productos
-- ============================================================

CREATE TABLE [dbo].[Categorias]
(
    [Id]          [int]          IDENTITY(1,1) NOT NULL,
    [Descripcion] [varchar](100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Marcas]
(
    [Id]          [int]          IDENTITY(1,1) NOT NULL,
    [Descripcion] [varchar](100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Productos]
(
    [Id]                [int]            IDENTITY(1,1) NOT NULL,
    [Codigo]            [varchar](50)    NOT NULL,
    [NombreProducto]    [varchar](150)   NOT NULL,
    [Descripcion]       [varchar](500)   NULL,
    [StockActual]       [int]            NOT NULL DEFAULT (0),
    [StockMinimo]       [int]            NOT NULL DEFAULT (0),
    [Precio]            [decimal](10,2)  NOT NULL DEFAULT (0),
    [PorcentajeGanancia][decimal](5,2)   NOT NULL DEFAULT (0),
    [IdMarca]           [int]            NOT NULL,
    [IdCategoria]       [int]            NOT NULL,
    [Estado]            [bit]            NOT NULL DEFAULT (1),
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

CREATE TABLE [dbo].[Clientes]
(
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

CREATE TABLE [dbo].[Proveedores]
(
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

CREATE TABLE [dbo].[Compras]
(
    [Id]          [int]            IDENTITY(1,1) NOT NULL,
    [Fecha]       [datetime]       NOT NULL DEFAULT (getdate()),
    [IdProveedor] [int]            NOT NULL,
    [IdUsuario]   [int]            NOT NULL,
    [Total]       [decimal](12,2)  NOT NULL DEFAULT (0),
    [Estado]      [bit]            NOT NULL DEFAULT (1),
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

CREATE TABLE [dbo].[DetalleCompras]
(
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

CREATE TABLE [dbo].[Facturas]
(
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

CREATE TABLE [dbo].[DetalleFacturas]
(
    [Id]                [int]           IDENTITY(1,1) NOT NULL,
    [IdFactura]         [int]           NOT NULL,
    [IdProducto]        [int]           NOT NULL,
    [Cantidad]          [int]           NOT NULL,
    [PrecioCompra]      [decimal](10,2) NOT NULL,
    [PorcentajeGanancia][decimal](5,2)  NOT NULL,
    [PrecioVenta]       [decimal](10,2) NOT NULL,
    [Subtotal]          AS ([Cantidad] * [PrecioVenta]) PERSISTED,
    [IdLote]            [int]           NULL,
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

-- ============================================================
-- LOTES (trazabilidad de stock por lote de compra)
-- ============================================================

CREATE TABLE [dbo].[Lotes]
(
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

ALTER TABLE [dbo].[DetalleFacturas] ADD CONSTRAINT [FK_DetalleFacturas_Lote]
    FOREIGN KEY([IdLote]) REFERENCES [dbo].[Lotes] ([Id])
GO

-- ============================================================
-- STORED PROCEDURES - MARCAS
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Marcas_Listar]
AS
BEGIN
    SELECT Id, Descripcion FROM [dbo].[Marcas]
END
GO

CREATE PROCEDURE [dbo].[SP_Marcas_Insertar]
    @Descripcion varchar(100)
AS
BEGIN
    INSERT INTO [dbo].[Marcas] (Descripcion) VALUES (@Descripcion)
END
GO

CREATE PROCEDURE [dbo].[SP_Marcas_Actualizar]
    @Id int,
    @Descripcion varchar(100)
AS
BEGIN
    UPDATE [dbo].[Marcas] SET Descripcion = @Descripcion WHERE Id = @Id
END
GO

CREATE PROCEDURE [dbo].[SP_Marcas_Eliminar]
    @Id int
AS
BEGIN
    DELETE FROM [dbo].[Marcas] WHERE Id = @Id
END
GO

-- ============================================================
-- STORED PROCEDURES - CATEGORIAS
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Categorias_Listar]
AS
BEGIN
    SELECT Id, Descripcion FROM [dbo].[Categorias]
END
GO

CREATE PROCEDURE [dbo].[SP_Categorias_Insertar]
    @Descripcion varchar(100)
AS
BEGIN
    INSERT INTO [dbo].[Categorias] (Descripcion) VALUES (@Descripcion)
END
GO

CREATE PROCEDURE [dbo].[SP_Categorias_Actualizar]
    @Id int,
    @Descripcion varchar(100)
AS
BEGIN
    UPDATE [dbo].[Categorias] SET Descripcion = @Descripcion WHERE Id = @Id
END
GO

CREATE PROCEDURE [dbo].[SP_Categorias_Eliminar]
    @Id int
AS
BEGIN
    DELETE FROM [dbo].[Categorias] WHERE Id = @Id
END
GO

-- ============================================================
-- STORED PROCEDURES - PRODUCTOS
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Productos_Listar]
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
END
GO

CREATE PROCEDURE [dbo].[SP_Productos_Insertar]
    @Codigo             varchar(50),
    @NombreProducto     varchar(150),
    @Descripcion        varchar(500) = NULL,
    @Precio             decimal(10,2),
    @StockMinimo        int,
    @PorcentajeGanancia decimal(5,2),
    @IdMarca            int,
    @IdCategoria        int
AS
BEGIN
    INSERT INTO [dbo].[Productos]
        (Codigo, NombreProducto, Descripcion, Precio, StockMinimo, PorcentajeGanancia, IdMarca, IdCategoria)
    VALUES
        (@Codigo, @NombreProducto, @Descripcion, @Precio, @StockMinimo, @PorcentajeGanancia, @IdMarca, @IdCategoria)
END
GO

CREATE PROCEDURE [dbo].[SP_Productos_Actualizar]
    @Id                 int,
    @Codigo             varchar(50),
    @NombreProducto     varchar(150),
    @Descripcion        varchar(500) = NULL,
    @Precio             decimal(10,2),
    @StockMinimo        int,
    @PorcentajeGanancia decimal(5,2),
    @IdMarca            int,
    @IdCategoria        int
AS
BEGIN
    UPDATE [dbo].[Productos]
    SET Codigo             = @Codigo,
        NombreProducto     = @NombreProducto,
        Descripcion        = @Descripcion,
        Precio             = @Precio,
        StockMinimo        = @StockMinimo,
        PorcentajeGanancia = @PorcentajeGanancia,
        IdMarca            = @IdMarca,
        IdCategoria        = @IdCategoria
    WHERE Id = @Id
END
GO

CREATE PROCEDURE [dbo].[SP_Productos_BajaLogica]
    @Id int
AS
BEGIN
    UPDATE [dbo].[Productos] SET Estado = 0 WHERE Id = @Id
END
GO

-- ============================================================
-- STORED PROCEDURES - CLIENTES
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Clientes_Listar]
AS
BEGIN
    SELECT * FROM [dbo].[Clientes]
END
GO

CREATE PROCEDURE [dbo].[SP_Clientes_Insertar]
(
    @DNI       varchar(15),
    @Nombre    varchar(100),
    @Apellido  varchar(100),
    @Direccion varchar(200),
    @Telefono  varchar(20),
    @Email     varchar(150),
    @Estado    bit
)
AS
BEGIN
    INSERT INTO [dbo].[Clientes] (DNI, Nombre, Apellido, Direccion, Telefono, Email, Estado)
    VALUES (@DNI, @Nombre, @Apellido, @Direccion, @Telefono, @Email, 1)
END
GO

CREATE PROCEDURE [dbo].[SP_Clientes_Actualizar]
(
    @ID        int,
    @DNI       varchar(15),
    @Nombre    varchar(100),
    @Apellido  varchar(100),
    @Direccion varchar(200),
    @Telefono  varchar(20),
    @Email     varchar(150)
)
AS
BEGIN
    UPDATE [dbo].[Clientes]
    SET DNI       = @DNI,
        Nombre    = @Nombre,
        Apellido  = @Apellido,
        Direccion = @Direccion,
        Telefono  = @Telefono,
        Email     = @Email
    WHERE ID = @ID
END
GO

CREATE PROCEDURE [dbo].[SP_Clientes_BajaLogica]
    @ID int
AS
BEGIN
    UPDATE [dbo].[Clientes] SET Estado = 0 WHERE ID = @ID
END
GO

-- ============================================================
-- STORED PROCEDURES - PROVEEDORES
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Proveedores_Listar]
AS
BEGIN
    SELECT ID, RazonSocial, Cuit, Direccion, Telefono, Email, Estado
    FROM [dbo].[Proveedores]
    WHERE Estado = 1
END
GO

CREATE PROCEDURE [dbo].[SP_Proveedores_Insertar]
    @RazonSocial varchar(150),
    @Cuit        varchar(20),
    @Direccion   varchar(200) = NULL,
    @Telefono    varchar(20)  = NULL,
    @Email       varchar(150) = NULL
AS
BEGIN
    INSERT INTO [dbo].[Proveedores] (RazonSocial, Cuit, Direccion, Telefono, Email)
    VALUES (@RazonSocial, @Cuit, @Direccion, @Telefono, @Email)
END
GO

CREATE PROCEDURE [dbo].[SP_Proveedores_Actualizar]
    @ID          int,
    @RazonSocial varchar(150),
    @Cuit        varchar(20),
    @Direccion   varchar(200) = NULL,
    @Telefono    varchar(20)  = NULL,
    @Email       varchar(150) = NULL
AS
BEGIN
    UPDATE [dbo].[Proveedores]
    SET RazonSocial = @RazonSocial,
        Cuit        = @Cuit,
        Direccion   = @Direccion,
        Telefono    = @Telefono,
        Email       = @Email
    WHERE ID = @ID
END
GO

CREATE PROCEDURE [dbo].[SP_Proveedores_BajaLogica]
    @ID int
AS
BEGIN
    UPDATE [dbo].[Proveedores] SET Estado = 0 WHERE ID = @ID
END
GO

-- ============================================================
-- STORED PROCEDURES - COMPRAS
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Compras_Listar]
AS
BEGIN
    SELECT
        c.Id,
        c.Fecha,
        c.IdProveedor,
        p.RazonSocial AS Proveedor,
        c.IdUsuario,
        c.Total,
        c.Estado
    FROM [dbo].[Compras] c
    INNER JOIN [dbo].[Proveedores] p ON c.IdProveedor = p.ID
    WHERE c.Estado = 1
    ORDER BY c.Fecha DESC
END
GO

CREATE PROCEDURE [dbo].[SP_Compras_Insertar]
    @IdProveedor int,
    @IdUsuario   int,
    @NewId       int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[Compras] (IdProveedor, IdUsuario, Total)
    VALUES (@IdProveedor, @IdUsuario, 0)
    SET @NewId = SCOPE_IDENTITY()
END
GO

CREATE PROCEDURE [dbo].[SP_Compras_ActualizarTotal]
    @IdCompra int
AS
BEGIN
    UPDATE [dbo].[Compras]
    SET Total = (
        SELECT ISNULL(SUM(Subtotal), 0)
        FROM [dbo].[DetalleCompras]
        WHERE IdCompra = @IdCompra
    )
    WHERE Id = @IdCompra
END
GO

CREATE PROCEDURE [dbo].[SP_DetalleCompras_Insertar]
    @IdCompra       int,
    @IdProducto     int,
    @Cantidad       int,
    @PrecioUnitario decimal(10,2),
    @NewId          int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[DetalleCompras] (IdCompra, IdProducto, Cantidad, PrecioUnitario)
    VALUES (@IdCompra, @IdProducto, @Cantidad, @PrecioUnitario)
    SET @NewId = SCOPE_IDENTITY()
END
GO

-- ============================================================
-- STORED PROCEDURES - LOTES
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Lotes_Crear]
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

CREATE PROCEDURE [dbo].[SP_Lotes_ListarPorProducto]
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

CREATE PROCEDURE [dbo].[SP_Lotes_DescontarStock]
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

-- ============================================================
-- STORED PROCEDURES - PERFILES
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Perfiles_Listar]
AS
BEGIN
    SELECT Id, NombrePerfil FROM [dbo].[Perfiles] WHERE Estado = 1
END
GO

-- ============================================================
-- STORED PROCEDURES - USUARIOS
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Usuarios_Login]
    @User      varchar(50),
    @Contrasena varchar(256)
AS
BEGIN
    SELECT
        U.[Id],
        U.[User],
        U.[Contrasena],
        U.[Estado],
        P.Id        AS IdPerfil,
        P.NombrePerfil
    FROM [dbo].[Usuarios] U
    INNER JOIN [dbo].[Perfiles] P ON U.IdPerfil = P.Id
    WHERE U.[User]      = @User
      AND U.Contrasena  = @Contrasena
      AND U.Estado      = 1
END
GO

-- Alta de usuario desde el formulario de alta (UI Usuarios.aspx)
CREATE PROCEDURE [dbo].[SP_Usuarios_Agregar]
    @User      varchar(50),
    @Contrasena varchar(256),
    @IdPerfil  int,
    @Estado    bit
AS
BEGIN
    INSERT INTO [dbo].[Usuarios] ([User], Contrasena, IdPerfil, Estado)
    VALUES (@User, @Contrasena, @IdPerfil, @Estado)
END
GO

-- Cambio de contraseña validando la contraseña actual
CREATE PROCEDURE [dbo].[SP_Usuarios_CambiarContrasena]
    @Id              int,
    @ContrasenaActual varchar(256),
    @ContrasenaNueva  varchar(256)
AS
BEGIN
    UPDATE [dbo].[Usuarios]
    SET Contrasena = @ContrasenaNueva
    WHERE Id          = @Id
      AND Contrasena  = @ContrasenaActual
      AND Estado      = 1
END
GO

-- Actualizar usuario y perfil (gestión de usuarios - administrador)
CREATE PROCEDURE [dbo].[SP_Usuarios_Actualizar]
    @Id       int,
    @User     varchar(50),
    @IdPerfil int
AS
BEGIN
    UPDATE [dbo].[Usuarios]
    SET [User]   = @User,
        IdPerfil = @IdPerfil
    WHERE Id = @Id
END
GO

-- Actualizar solo la contraseña (sin validar la anterior - uso administrativo)
CREATE PROCEDURE [dbo].[SP_Usuarios_ActualizarContrasena]
    @Id            int,
    @ContrasenaHash varchar(256)
AS
BEGIN
    UPDATE [dbo].[Usuarios] SET Contrasena = @ContrasenaHash WHERE Id = @Id
END
GO

CREATE PROCEDURE [dbo].[SP_Usuarios_BajaLogica]
    @Id int
AS
BEGIN
    UPDATE [dbo].[Usuarios] SET Estado = 0 WHERE Id = @Id
END
GO

-- Insertar usuario desde gestión administrativa (sin @Estado, estado por defecto 1)
CREATE PROCEDURE [dbo].[SP_Usuarios_Insertar]
    @User           varchar(50),
    @ContrasenaHash varchar(256),
    @IdPerfil       int
AS
BEGIN
    INSERT INTO [dbo].[Usuarios] ([User], Contrasena, IdPerfil)
    VALUES (@User, @ContrasenaHash, @IdPerfil)
END
GO

CREATE PROCEDURE [dbo].[SP_Usuarios_Listar]
AS
BEGIN
    SELECT u.Id, u.[User], u.IdPerfil, p.NombrePerfil, u.Estado
    FROM [dbo].[Usuarios] u
    INNER JOIN [dbo].[Perfiles] p ON u.IdPerfil = p.Id
    WHERE u.Estado = 1
END
GO

-- ============================================================
-- DATOS DE PRUEBA (seed)
-- ============================================================

-- Perfiles (Ids explícitos: 1=Administrador, 2=Vendedor, 3=Cliente, 4=Proveedor)
SET IDENTITY_INSERT [dbo].[Perfiles] ON
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Perfiles] WHERE Id = 1)
    INSERT INTO [dbo].[Perfiles] (Id, NombrePerfil) VALUES (1, 'Administrador')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Perfiles] WHERE Id = 2)
    INSERT INTO [dbo].[Perfiles] (Id, NombrePerfil) VALUES (2, 'Vendedor')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Perfiles] WHERE Id = 3)
    INSERT INTO [dbo].[Perfiles] (Id, NombrePerfil) VALUES (3, 'Cliente')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Perfiles] WHERE Id = 4)
    INSERT INTO [dbo].[Perfiles] (Id, NombrePerfil) VALUES (4, 'Proveedor')
GO
SET IDENTITY_INSERT [dbo].[Perfiles] OFF
GO

-- Usuarios
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [User] = 'admin')
    INSERT INTO [dbo].[Usuarios] ([User], Contrasena, IdPerfil) VALUES ('admin', 'admin123', 1)
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [User] = 'vendedor')
    INSERT INTO [dbo].[Usuarios] ([User], Contrasena, IdPerfil) VALUES ('vendedor', 'venta123', 2)
GO

-- Catalogo
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categorias] WHERE Descripcion = 'Indumentaria')
    INSERT INTO [dbo].[Categorias] (Descripcion) VALUES ('Indumentaria')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Marcas] WHERE Descripcion = 'Nike')
    INSERT INTO [dbo].[Marcas] (Descripcion) VALUES ('Nike')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Productos] WHERE Codigo = 'P0001')
    INSERT INTO [dbo].[Productos]
        (Codigo, NombreProducto, Descripcion, StockActual, StockMinimo, Precio, PorcentajeGanancia, IdMarca, IdCategoria)
    VALUES
        ('P0001', 'Zapatillas Running', 'Zapatillas para running de uso diario', 0, 10, 8500.00, 35.00, 1, 1)
GO

-- Cliente de prueba
IF NOT EXISTS (SELECT 1 FROM [dbo].[Clientes] WHERE DNI = '12345678')
    INSERT INTO [dbo].[Clientes] (DNI, Nombre, Apellido, Direccion, Telefono, Email, Estado)
    VALUES ('12345678', 'Juan', 'Perez', 'Calle 123', '1122334455', 'juan@test.com', 1)
GO

-- Proveedor de prueba
IF NOT EXISTS (SELECT 1 FROM [dbo].[Proveedores] WHERE Cuit = '30-12345678-9')
    INSERT INTO [dbo].[Proveedores] (RazonSocial, Cuit, Direccion, Telefono, Email)
    VALUES ('Distribuidora Mayorista S.A.', '30-12345678-9', 'Av. Corrientes 1234', '011-4567-8901', 'ventas@distribuidora.com')
GO
