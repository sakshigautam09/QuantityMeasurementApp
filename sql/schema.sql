CREATE DATABASE QuantityMeasurementDb;

USE QuantityMeasurementDb;

-- ── Primary operations table ──────────────────────────────────────────────────
CREATE TABLE dbo.quantity_measurement_entity (
    id               UNIQUEIDENTIFIER   NOT NULL  DEFAULT NEWID(),
    timestamp        DATETIME2(7)       NOT NULL  DEFAULT SYSUTCDATETIME(),
    operation        NVARCHAR(20)       NOT NULL,   -- Compare|Convert|Add|Subtract|Divide
    measurement_type NVARCHAR(20)       NOT NULL,   -- Length|Weight|Volume|Temperature

    -- First operand
    first_value      FLOAT              NOT NULL,
    first_unit       NVARCHAR(20)       NOT NULL,

    -- Second operand (nullable – single-operand Convert stores NULL)
    second_value     FLOAT              NULL,
    second_unit      NVARCHAR(20)       NULL,

    -- Target unit (nullable – populated for "Add/Subtract with target" ops)
    target_unit      NVARCHAR(20)       NULL,

    result_display   NVARCHAR(200)      NOT NULL,
    has_error        BIT                NOT NULL  DEFAULT 0,
    error_message    NVARCHAR(500)      NULL,

    CONSTRAINT PK_quantity_measurement_entity PRIMARY KEY (id)
);
GO

-- ── Indexes for common query patterns ────────────────────────────────────────
CREATE NONCLUSTERED INDEX IX_qme_operation
    ON dbo.quantity_measurement_entity (operation);

CREATE NONCLUSTERED INDEX IX_qme_measurement_type
    ON dbo.quantity_measurement_entity (measurement_type);

CREATE NONCLUSTERED INDEX IX_qme_timestamp
    ON dbo.quantity_measurement_entity (timestamp DESC);
GO

-- ── Audit / history table (append-only mirror) ───────────────────────────────
CREATE TABLE dbo.quantity_measurement_history (
    history_id       BIGINT             NOT NULL  IDENTITY(1,1),
    entity_id        UNIQUEIDENTIFIER   NOT NULL,
    recorded_at      DATETIME2(7)       NOT NULL  DEFAULT SYSUTCDATETIME(),
    operation        NVARCHAR(20)       NOT NULL,
    measurement_type NVARCHAR(20)       NOT NULL,
    first_value      FLOAT              NOT NULL,
    first_unit       NVARCHAR(20)       NOT NULL,
    second_value     FLOAT              NULL,
    second_unit      NVARCHAR(20)       NULL,
    target_unit      NVARCHAR(20)       NULL,
    result_display   NVARCHAR(200)      NOT NULL,
    has_error        BIT                NOT NULL,

    CONSTRAINT PK_quantity_measurement_history PRIMARY KEY (history_id)
);
GO

-- ── Trigger: auto-populate history on every INSERT ───────────────────────────
CREATE OR ALTER TRIGGER trg_qme_after_insert
ON dbo.quantity_measurement_entity
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.quantity_measurement_history
        (entity_id, operation, measurement_type,
         first_value, first_unit, second_value, second_unit,
         target_unit, result_display, has_error)
    SELECT
        id, operation, measurement_type,
        first_value, first_unit, second_value, second_unit,
        target_unit, result_display, has_error
    FROM inserted;
END;
GO

SELECT * FROM quantity_measurement_entity;