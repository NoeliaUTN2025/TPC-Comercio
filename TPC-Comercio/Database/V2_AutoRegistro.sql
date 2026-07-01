-- ============================================================
-- Migración V2: Auto-Registro y Navegación por Roles
-- Base de datos: tpc_P3
-- Ejecutar contra la BD existente (NO es un script de creación)
-- ============================================================

USE [tpc_P3]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- 1. Vincular Usuarios con su entidad de negocio (Cliente/Proveedor)
-- ============================================================

ALTER TABLE [dbo].[Usuarios]
    ADD [IdEntidad] INT NOT NULL DEFAULT 0;
GO

-- ============================================================
-- 2. SP_Clientes_Insertar: devolver el nuevo ID generado
-- ============================================================

ALTER PROCEDURE [dbo].[SP_Clientes_Insertar]
(
    @DNI       varchar(15),
    @Nombre    varchar(100),
    @Apellido  varchar(100),
    @Direccion varchar(200),
    @Telefono  varchar(20),
    @Email     varchar(150),
    @Estado    bit,
    @NewId     int OUTPUT
)
AS
BEGIN
    INSERT INTO [dbo].[Clientes] (DNI, Nombre, Apellido, Direccion, Telefono, Email, Estado)
    VALUES (@DNI, @Nombre, @Apellido, @Direccion, @Telefono, @Email, 1)
    SET @NewId = SCOPE_IDENTITY()
END
GO

-- ============================================================
-- 3. SP_Proveedores_Insertar: devolver el nuevo ID generado
-- ============================================================

ALTER PROCEDURE [dbo].[SP_Proveedores_Insertar]
    @RazonSocial varchar(150),
    @Cuit        varchar(20),
    @Direccion   varchar(200) = NULL,
    @Telefono    varchar(20)  = NULL,
    @Email       varchar(150) = NULL,
    @NewId       int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[Proveedores] (RazonSocial, Cuit, Direccion, Telefono, Email)
    VALUES (@RazonSocial, @Cuit, @Direccion, @Telefono, @Email)
    SET @NewId = SCOPE_IDENTITY()
END
GO

-- ============================================================
-- 4. SP_Usuarios_Agregar: aceptar IdEntidad opcional
-- ============================================================

ALTER PROCEDURE [dbo].[SP_Usuarios_Agregar]
    @User       varchar(50),
    @Contrasena varchar(256),
    @IdPerfil   int,
    @Estado     bit,
    @IdEntidad  int = 0
AS
BEGIN
    INSERT INTO [dbo].[Usuarios] ([User], Contrasena, IdPerfil, Estado, IdEntidad)
    VALUES (@User, @Contrasena, @IdPerfil, @Estado, @IdEntidad)
END
GO

-- ============================================================
-- 5. SP_Usuarios_Login: incluir IdEntidad en el resultado
-- ============================================================

ALTER PROCEDURE [dbo].[SP_Usuarios_Login]
    @User       varchar(50),
    @Contrasena varchar(256)
AS
BEGIN
    SELECT
        U.[Id],
        U.[User],
        U.[Contrasena],
        U.[Estado],
        U.[IdEntidad],
        P.Id        AS IdPerfil,
        P.NombrePerfil
    FROM [dbo].[Usuarios] U
    INNER JOIN [dbo].[Perfiles] P ON U.IdPerfil = P.Id
    WHERE U.[User]      = @User
      AND U.Contrasena  = @Contrasena
      AND U.Estado      = 1
END
GO

-- ============================================================
-- 6. Listar compras de un proveedor específico (para portal proveedor)
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Compras_ListarPorProveedor]
    @IdProveedor int
AS
BEGIN
    SELECT
        c.Id,
        c.Fecha,
        c.Total,
        ISNULL((SELECT SUM(dc.Cantidad) FROM DetalleCompras dc WHERE dc.IdCompra = c.Id), 0) AS CantidadTotal
    FROM Compras c
    WHERE c.IdProveedor = @IdProveedor
      AND c.Estado = 1
    ORDER BY c.Fecha DESC
END
GO

-- ============================================================
-- 7. Tabla de propuestas de lotes por proveedor
-- ============================================================

CREATE TABLE [dbo].[PropuestasProveedor]
(
    [Id]            [int]            IDENTITY(1,1) NOT NULL,
    [IdProveedor]   [int]            NOT NULL,
    [IdProducto]    [int]            NOT NULL,
    [Cantidad]      [int]            NOT NULL,
    [PrecioUnitario][decimal](18,2)  NOT NULL,
    [Estado]        [varchar](20)    NOT NULL DEFAULT 'Pendiente',
    [Fecha]         [datetime]       NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Propuestas_Proveedor] FOREIGN KEY ([IdProveedor]) REFERENCES [dbo].[Proveedores] ([ID]),
    CONSTRAINT [FK_Propuestas_Producto]  FOREIGN KEY ([IdProducto])  REFERENCES [dbo].[Productos]   ([Id])
)
GO

-- ============================================================
-- 8. SPs de PropuestasProveedor
-- ============================================================

CREATE PROCEDURE [dbo].[SP_Propuestas_Insertar]
    @IdProveedor   int,
    @IdProducto    int,
    @Cantidad      int,
    @PrecioUnitario decimal(18,2)
AS
BEGIN
    INSERT INTO [dbo].[PropuestasProveedor] (IdProveedor, IdProducto, Cantidad, PrecioUnitario)
    VALUES (@IdProveedor, @IdProducto, @Cantidad, @PrecioUnitario)
END
GO

CREATE PROCEDURE [dbo].[SP_Propuestas_ListarPorProveedor]
    @IdProveedor int
AS
BEGIN
    SELECT pp.Id, pp.Cantidad, pp.PrecioUnitario, pp.Estado, pp.Fecha,
           p.NombreProducto
    FROM PropuestasProveedor pp
    INNER JOIN Productos p ON pp.IdProducto = p.Id
    WHERE pp.IdProveedor = @IdProveedor
    ORDER BY pp.Fecha DESC
END
GO

CREATE PROCEDURE [dbo].[SP_Propuestas_ListarPendientes]
AS
BEGIN
    SELECT pp.Id, pp.Cantidad, pp.PrecioUnitario, pp.Estado, pp.Fecha,
           p.Id AS IdProducto, p.NombreProducto,
           pr.ID AS IdProveedor, pr.RazonSocial
    FROM PropuestasProveedor pp
    INNER JOIN Productos   p  ON pp.IdProducto  = p.Id
    INNER JOIN Proveedores pr ON pp.IdProveedor = pr.ID
    WHERE pp.Estado = 'Pendiente'
    ORDER BY pp.Fecha ASC
END
GO

CREATE PROCEDURE [dbo].[SP_Propuestas_Aprobar]
    @Id int
AS
BEGIN
    UPDATE [dbo].[PropuestasProveedor]
    SET Estado = 'Aprobado'
    WHERE Id = @Id
END
GO

CREATE PROCEDURE [dbo].[SP_Propuestas_ObtenerPorId]
    @Id int
AS
BEGIN
    SELECT pp.Id, pp.IdProveedor, pp.IdProducto, pp.Cantidad, pp.PrecioUnitario, pp.Estado, pp.Fecha,
           p.NombreProducto, pr.RazonSocial
    FROM PropuestasProveedor pp
    INNER JOIN Productos   p  ON pp.IdProducto  = p.Id
    INNER JOIN Proveedores pr ON pp.IdProveedor = pr.ID
    WHERE pp.Id = @Id
END
GO
