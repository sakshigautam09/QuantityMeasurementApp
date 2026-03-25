-- ============================================================
-- UC16: Quantity Measurement Database Schema for SQL Server
-- Run this script once in SSMS before starting the application.
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuantityMeasurementDB')
BEGIN
    CREATE DATABASE QuantityMeasurementDB;
    PRINT 'Database QuantityMeasurementDB created.';
END
GO

USE QuantityMeasurementDB;
GO

-- ── Main measurements table ───────────────────────────────────
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'quantity_measurements'
)
BEGIN
    CREATE TABLE quantity_measurements
    (
        id                  INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        operation           NVARCHAR(50)   NOT NULL,
        [timestamp]         DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),

        operand1_value      FLOAT          NULL,
        operand1_unit       NVARCHAR(50)   NULL,
        operand1_category   NVARCHAR(50)   NULL,

        operand2_value      FLOAT          NULL,
        operand2_unit       NVARCHAR(50)   NULL,
        operand2_category   NVARCHAR(50)   NULL,

        result_value        FLOAT          NULL,
        result_unit         NVARCHAR(50)   NULL,
        result_category     NVARCHAR(50)   NULL,

        has_error           BIT            NOT NULL DEFAULT 0,
        error_message       NVARCHAR(500)  NULL
    );

    CREATE INDEX IX_qm_operation ON quantity_measurements (operation);
    CREATE INDEX IX_qm_category  ON quantity_measurements (operand1_category);

    PRINT 'Table quantity_measurements created.';
END
ELSE
    PRINT 'Table quantity_measurements already exists.';
GO

-- ── Audit / history table ─────────────────────────────────────
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'quantity_measurement_history'
)
BEGIN
    CREATE TABLE quantity_measurement_history
    (
        id               INT          NOT NULL IDENTITY(1,1) PRIMARY KEY,
        operation_type   NVARCHAR(50) NOT NULL,
        action           NVARCHAR(50) NOT NULL,
        action_timestamp DATETIME2    NOT NULL DEFAULT SYSUTCDATETIME()
    );
    PRINT 'Table quantity_measurement_history created.';
END
GO

-- ── Stored Procedures ─────────────────────────────────────────

-- sp_SaveMeasurement
IF OBJECT_ID('sp_SaveMeasurement','P') IS NOT NULL DROP PROCEDURE sp_SaveMeasurement;
GO
CREATE PROCEDURE sp_SaveMeasurement
    @operation          NVARCHAR(50),
    @timestamp          DATETIME2,
    @operand1_value     FLOAT         = NULL,
    @operand1_unit      NVARCHAR(50)  = NULL,
    @operand1_category  NVARCHAR(50)  = NULL,
    @operand2_value     FLOAT         = NULL,
    @operand2_unit      NVARCHAR(50)  = NULL,
    @operand2_category  NVARCHAR(50)  = NULL,
    @result_value       FLOAT         = NULL,
    @result_unit        NVARCHAR(50)  = NULL,
    @result_category    NVARCHAR(50)  = NULL,
    @has_error          BIT           = 0,
    @error_message      NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO quantity_measurements
        (operation, [timestamp],
         operand1_value, operand1_unit, operand1_category,
         operand2_value, operand2_unit, operand2_category,
         result_value,   result_unit,   result_category,
         has_error, error_message)
    VALUES
        (@operation, @timestamp,
         @operand1_value, @operand1_unit, @operand1_category,
         @operand2_value, @operand2_unit, @operand2_category,
         @result_value,   @result_unit,   @result_category,
         @has_error, @error_message);
END
GO

-- sp_GetAllMeasurements
IF OBJECT_ID('sp_GetAllMeasurements','P') IS NOT NULL DROP PROCEDURE sp_GetAllMeasurements;
GO
CREATE PROCEDURE sp_GetAllMeasurements
AS
BEGIN
    SET NOCOUNT ON;
    SELECT operation, [timestamp],
           operand1_value, operand1_unit, operand1_category,
           operand2_value, operand2_unit, operand2_category,
           result_value,   result_unit,   result_category,
           has_error, error_message
    FROM quantity_measurements
    ORDER BY [timestamp] DESC;
END
GO

-- sp_GetMeasurementsByOperation
IF OBJECT_ID('sp_GetMeasurementsByOperation','P') IS NOT NULL DROP PROCEDURE sp_GetMeasurementsByOperation;
GO
CREATE PROCEDURE sp_GetMeasurementsByOperation
    @operation NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT operation, [timestamp],
           operand1_value, operand1_unit, operand1_category,
           operand2_value, operand2_unit, operand2_category,
           result_value,   result_unit,   result_category,
           has_error, error_message
    FROM quantity_measurements
    WHERE operation = @operation
    ORDER BY [timestamp] DESC;
END
GO

-- sp_GetMeasurementsByCategory
IF OBJECT_ID('sp_GetMeasurementsByCategory','P') IS NOT NULL DROP PROCEDURE sp_GetMeasurementsByCategory;
GO
CREATE PROCEDURE sp_GetMeasurementsByCategory
    @category NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT operation, [timestamp],
           operand1_value, operand1_unit, operand1_category,
           operand2_value, operand2_unit, operand2_category,
           result_value,   result_unit,   result_category,
           has_error, error_message
    FROM quantity_measurements
    WHERE operand1_category = @category OR operand2_category = @category
    ORDER BY [timestamp] DESC;
END
GO

-- sp_GetTotalMeasurementCount
IF OBJECT_ID('sp_GetTotalMeasurementCount','P') IS NOT NULL DROP PROCEDURE sp_GetTotalMeasurementCount;
GO
CREATE PROCEDURE sp_GetTotalMeasurementCount
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM quantity_measurements;
END
GO

-- sp_DeleteAllMeasurements
IF OBJECT_ID('sp_DeleteAllMeasurements','P') IS NOT NULL DROP PROCEDURE sp_DeleteAllMeasurements;
GO
CREATE PROCEDURE sp_DeleteAllMeasurements
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM quantity_measurement_history;
    DELETE FROM quantity_measurements;
END
GO

PRINT 'UC16 schema and stored procedures installed successfully.';
GO

USE QuantityMeasurementDB;

-- All operations saved
SELECT * FROM quantity_measurements ORDER BY timestamp DESC;

-- Audit trail
SELECT * FROM quantity_measurement_history ORDER BY action_timestamp DESC;

-- Total count
SELECT COUNT(*) AS TotalRecords FROM quantity_measurements;

SELECT name FROM sys.procedures ORDER BY name;