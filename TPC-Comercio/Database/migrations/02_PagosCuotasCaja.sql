-- Script de migracion: Pagos, Cuotas y Caja
-- Ejecutar contra tpc_P3 en orden, despues de 01_Lotes.sql
USE [tpc_P3]
GO

-- 1. Tabla Pagos
CREATE TABLE [dbo].[Pagos] (
    [Id]             [int]           IDENTITY(1,1) NOT NULL,
    [IdFactura]      [int]           NOT NULL,
    [Tipo]           [varchar](20)   NOT NULL,
    [Monto]          [decimal](12,2) NOT NULL,
    [CantidadCuotas] [int]           NOT NULL DEFAULT (0),
    [Fecha]          [datetime]      NOT NULL DEFAULT (getdate()),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pagos_Factura] FOREIGN KEY([IdFactura]) REFERENCES [dbo].[Facturas] ([Id])
) ON [PRIMARY]
GO

-- 2. Tabla Cuotas
CREATE TABLE [dbo].[Cuotas] (
    [Id]          [int]           IDENTITY(1,1) NOT NULL,
    [IdPago]      [int]           NOT NULL,
    [NroCuota]    [int]           NOT NULL,
    [Monto]       [decimal](10,2) NOT NULL,
    [Interes]     [decimal](10,2) NOT NULL,
    [Vencimiento] [datetime]      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cuotas_Pago] FOREIGN KEY([IdPago]) REFERENCES [dbo].[Pagos] ([Id])
) ON [PRIMARY]
GO

-- 3. Tabla Caja
CREATE TABLE [dbo].[Caja] (
    [Id]                   [int]           IDENTITY(1,1) NOT NULL,
    [FechaApertura]        [datetime]      NOT NULL DEFAULT (getdate()),
    [MontoApertura]        [decimal](12,2) NOT NULL,
    [IdUsuarioApertura]    [int]           NOT NULL,
    [FechaCierre]          [datetime]      NULL,
    [MontoCierreDeclarado] [decimal](12,2) NULL,
    [MontoCierreCalculado] [decimal](12,2) NULL,
    [IdUsuarioCierre]      [int]           NULL,
    [Estado]               [bit]           NOT NULL DEFAULT (1),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Caja_UsuarioApertura] FOREIGN KEY([IdUsuarioApertura]) REFERENCES [dbo].[Usuarios] ([Id]),
    CONSTRAINT [FK_Caja_UsuarioCierre]   FOREIGN KEY([IdUsuarioCierre])   REFERENCES [dbo].[Usuarios] ([Id])
) ON [PRIMARY]
GO

-- 4. SPs de Pagos
CREATE OR ALTER PROCEDURE [dbo].[SP_Pagos_Crear]
    @IdFactura      int,
    @Tipo           varchar(20),
    @Monto          decimal(12,2),
    @CantidadCuotas int,
    @NewId          int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[Pagos] (IdFactura, Tipo, Monto, CantidadCuotas)
    VALUES (@IdFactura, @Tipo, @Monto, @CantidadCuotas)
    SET @NewId = SCOPE_IDENTITY()
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Pagos_ObtenerPorFactura]
    @IdFactura int
AS
BEGIN
    SELECT Id, IdFactura, Tipo, Monto, CantidadCuotas, Fecha
    FROM [dbo].[Pagos]
    WHERE IdFactura = @IdFactura
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Pagos_SumarEfectivoDesde]
    @Desde datetime
AS
BEGIN
    SELECT ISNULL(SUM(Monto), 0) AS Total
    FROM [dbo].[Pagos]
    WHERE Tipo = 'Efectivo' AND Fecha >= @Desde
END
GO

-- 5. SPs de Cuotas
CREATE OR ALTER PROCEDURE [dbo].[SP_Cuotas_Crear]
    @IdPago      int,
    @NroCuota    int,
    @Monto       decimal(10,2),
    @Interes     decimal(10,2),
    @Vencimiento datetime
AS
BEGIN
    INSERT INTO [dbo].[Cuotas] (IdPago, NroCuota, Monto, Interes, Vencimiento)
    VALUES (@IdPago, @NroCuota, @Monto, @Interes, @Vencimiento)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Cuotas_ListarPorPago]
    @IdPago int
AS
BEGIN
    SELECT Id, IdPago, NroCuota, Monto, Interes, Vencimiento
    FROM [dbo].[Cuotas]
    WHERE IdPago = @IdPago
    ORDER BY NroCuota ASC
END
GO

-- 6. SPs de Caja
CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_Abrir]
    @MontoApertura decimal(12,2),
    @IdUsuario     int,
    @NewId         int OUTPUT
AS
BEGIN
    INSERT INTO [dbo].[Caja] (MontoApertura, IdUsuarioApertura)
    SELECT @MontoApertura, @IdUsuario
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Caja] WITH (UPDLOCK, HOLDLOCK) WHERE Estado = 1)

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('Ya hay una caja abierta.', 16, 1)
        RETURN
    END

    SET @NewId = SCOPE_IDENTITY()
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_ObtenerAbierta]
AS
BEGIN
    SELECT
        c.Id, c.FechaApertura, c.MontoApertura, c.IdUsuarioApertura, ua.[User] AS UsuarioApertura,
        c.FechaCierre, c.MontoCierreDeclarado, c.MontoCierreCalculado, c.IdUsuarioCierre, uc.[User] AS UsuarioCierre,
        c.Estado
    FROM [dbo].[Caja] c
    INNER JOIN [dbo].[Usuarios] ua ON c.IdUsuarioApertura = ua.Id
    LEFT JOIN  [dbo].[Usuarios] uc ON c.IdUsuarioCierre   = uc.Id
    WHERE c.Estado = 1
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_Cerrar]
    @Id                   int,
    @MontoCierreDeclarado decimal(12,2),
    @IdUsuario            int
AS
BEGIN
    DECLARE @FechaApertura datetime
    DECLARE @MontoCalculado decimal(12,2)

    SELECT @FechaApertura = FechaApertura FROM [dbo].[Caja] WHERE Id = @Id AND Estado = 1

    IF @FechaApertura IS NULL
    BEGIN
        RAISERROR('No se encontro una caja abierta con el Id indicado.', 16, 1)
        RETURN
    END

    SELECT @MontoCalculado = ISNULL(SUM(Monto), 0)
    FROM [dbo].[Pagos]
    WHERE Tipo = 'Efectivo' AND Fecha BETWEEN @FechaApertura AND getdate()

    UPDATE [dbo].[Caja]
    SET FechaCierre = getdate(),
        MontoCierreDeclarado = @MontoCierreDeclarado,
        MontoCierreCalculado = @MontoCalculado,
        IdUsuarioCierre = @IdUsuario,
        Estado = 0
    WHERE Id = @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Caja_Listar]
AS
BEGIN
    SELECT
        c.Id, c.FechaApertura, c.MontoApertura, c.IdUsuarioApertura, ua.[User] AS UsuarioApertura,
        c.FechaCierre, c.MontoCierreDeclarado, c.MontoCierreCalculado, c.IdUsuarioCierre, uc.[User] AS UsuarioCierre,
        c.Estado
    FROM [dbo].[Caja] c
    INNER JOIN [dbo].[Usuarios] ua ON c.IdUsuarioApertura = ua.Id
    LEFT JOIN  [dbo].[Usuarios] uc ON c.IdUsuarioCierre   = uc.Id
    ORDER BY c.FechaApertura DESC
END
GO
