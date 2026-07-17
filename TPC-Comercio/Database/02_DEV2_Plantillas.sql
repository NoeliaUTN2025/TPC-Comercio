-- =============================================
-- Crear tabla de Plantillas
-- =============================================
CREATE TABLE ReportePlantillas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Entidad VARCHAR(50) NOT NULL,
    FechaDesde DATE NULL,
    FechaHasta DATE NULL
)
GO

-- =============================================
-- SP Insertar Plantilla
-- =============================================
CREATE PROCEDURE SP_Plantillas_Insertar
    @Nombre VARCHAR(100),
    @Entidad VARCHAR(50),
    @FechaDesde DATE,
    @FechaHasta DATE
AS
BEGIN
    INSERT INTO ReportePlantillas (Nombre, Entidad, FechaDesde, FechaHasta)
    VALUES (@Nombre, @Entidad, @FechaDesde, @FechaHasta)
END
GO

-- =============================================
-- SP Listar Plantillas
-- =============================================
CREATE PROCEDURE SP_Plantillas_Listar
AS
BEGIN
    SELECT Id, Nombre, Entidad, FechaDesde, FechaHasta
    FROM ReportePlantillas
    ORDER BY Nombre ASC
END
GO
