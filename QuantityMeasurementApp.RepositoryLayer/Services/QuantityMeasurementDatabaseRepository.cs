// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : QuantityMeasurementDatabaseRepository.cs
// UC-16   : Database Integration
//
// Reads connection string from appsettings.json.
// Microsoft.Data.SqlClient has built-in connection pooling —
// no manual pool class needed. Every new SqlConnection()
// call reuses a pooled connection automatically.
//
// NuGet packages required:
//   dotnet add package Microsoft.Data.SqlClient
//   dotnet add package Microsoft.Extensions.Configuration
//   dotnet add package Microsoft.Extensions.Configuration.Json
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly string _connectionString;

        public QuantityMeasurementDatabaseRepository()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("QuantityMeasurementDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'QuantityMeasurementDb' not found in appsettings.json.");
        }

        // Overload for testing
        public QuantityMeasurementDatabaseRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            _connectionString = connectionString;
        }

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            const string sql = @"
                INSERT INTO dbo.quantity_measurement_entity
                    (id, timestamp, operation, measurement_type,
                     first_value, first_unit, second_value, second_unit,
                     target_unit, result_display, has_error, error_message)
                VALUES
                    (@id, @timestamp, @operation, @measurementType,
                     @firstValue, @firstUnit, @secondValue, @secondUnit,
                     @targetUnit, @resultDisplay, @hasError, @errorMessage)";

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id",              entity.Id);
                cmd.Parameters.AddWithValue("@timestamp",       entity.Timestamp);
                cmd.Parameters.AddWithValue("@operation",       entity.Operation.ToString());
                cmd.Parameters.AddWithValue("@measurementType", entity.FirstOperand.Type.ToString());
                cmd.Parameters.AddWithValue("@firstValue",      entity.FirstOperand.Value);
                cmd.Parameters.AddWithValue("@firstUnit",       entity.FirstOperand.UnitLabel);
                cmd.Parameters.AddWithValue("@secondValue",     (object?)entity.SecondOperand?.Value     ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@secondUnit",      (object?)entity.SecondOperand?.UnitLabel ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@targetUnit",      (object?)entity.TargetUnit?.UnitLabel    ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@resultDisplay",   entity.ResultDisplay);
                cmd.Parameters.AddWithValue("@hasError",        entity.HasError);
                cmd.Parameters.AddWithValue("@errorMessage",    (object?)entity.ErrorMessage ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Save failed: {ex.Message}", ex);
            }
        }

        public QuantityMeasurementEntity? FindById(Guid id)
        {
            const string sql =
                "SELECT * FROM dbo.quantity_measurement_entity WHERE id = @id";
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var reader = cmd.ExecuteReader();
                return reader.Read() ? MapRow(reader) : null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"FindById failed: {ex.Message}", ex);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindAll()
            => ExecuteQuery(
                "SELECT * FROM dbo.quantity_measurement_entity ORDER BY timestamp DESC");

        public IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
            QuantityMeasurementEntity.OperationType operation)
        {
            const string sql = @"SELECT * FROM dbo.quantity_measurement_entity
                                 WHERE operation = @operation ORDER BY timestamp DESC";
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@operation", operation.ToString());
                return ReadAll(cmd);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"FindByOperation failed: {ex.Message}", ex);
            }
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(string measurementType)
        {
            const string sql = @"SELECT * FROM dbo.quantity_measurement_entity
                                 WHERE measurement_type = @measurementType ORDER BY timestamp DESC";
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@measurementType", measurementType);
                return ReadAll(cmd);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"FindByMeasurementType failed: {ex.Message}", ex);
            }
        }

        public int GetTotalCount()
            => ExecuteScalar("SELECT COUNT(*) FROM dbo.quantity_measurement_entity");

        public int GetCountByOperation(QuantityMeasurementEntity.OperationType operation)
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.quantity_measurement_entity WHERE operation = @op";
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@op", operation.ToString());
                return (int)cmd.ExecuteScalar()!;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"GetCountByOperation failed: {ex.Message}", ex);
            }
        }

        public int GetErrorCount()
            => ExecuteScalar(
                "SELECT COUNT(*) FROM dbo.quantity_measurement_entity WHERE has_error = 1");

        public void Clear()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using (var cmd = new SqlCommand(
                    "DELETE FROM dbo.quantity_measurement_history", conn))
                    cmd.ExecuteNonQuery();
                using (var cmd = new SqlCommand(
                    "DELETE FROM dbo.quantity_measurement_entity", conn))
                    cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Clear failed: {ex.Message}", ex);
            }
        }

        public void ReleaseResources()
        {
            // SqlClient manages pooling automatically.
            // Call ClearAllPools() only if you need to force-drain on shutdown.
            SqlConnection.ClearAllPools();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────────

        private IReadOnlyList<QuantityMeasurementEntity> ExecuteQuery(string sql)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                return ReadAll(cmd);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Query failed: {ex.Message}", ex);
            }
        }

        private static List<QuantityMeasurementEntity> ReadAll(SqlCommand cmd)
        {
            var list = new List<QuantityMeasurementEntity>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapRow(reader));
            return list;
        }

        private int ExecuteScalar(string sql)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(sql, conn);
                return (int)cmd.ExecuteScalar()!;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"Scalar query failed: {ex.Message}", ex);
            }
        }

        private static QuantityMeasurementEntity MapRow(SqlDataReader r)
        {
            var    op              = Enum.Parse<QuantityMeasurementEntity.OperationType>(r.GetString(r.GetOrdinal("operation")), true);
            string measurementType = r.GetString(r.GetOrdinal("measurement_type"));
            var    firstDto        = BuildDTO(measurementType, r.GetDouble(r.GetOrdinal("first_value")), r.GetString(r.GetOrdinal("first_unit")));

            QuantityDTO? secondDto = null;
            int secOrd = r.GetOrdinal("second_value");
            if (!r.IsDBNull(secOrd))
                secondDto = BuildDTO(measurementType, r.GetDouble(secOrd), r.GetString(r.GetOrdinal("second_unit")));

            QuantityDTO? targetDto = null;
            int targetOrd = r.GetOrdinal("target_unit");
            if (!r.IsDBNull(targetOrd))
                targetDto = BuildDTO(measurementType, 0.0, r.GetString(targetOrd));

            string result   = r.GetString(r.GetOrdinal("result_display"));
            bool   hasError = r.GetBoolean(r.GetOrdinal("has_error"));

            if (hasError)
            {
                string err = r.IsDBNull(r.GetOrdinal("error_message")) ? "Unknown error" : r.GetString(r.GetOrdinal("error_message"));
                return new QuantityMeasurementEntity(op, firstDto, secondDto, err, true);
            }

            if (secondDto is null)
                return new QuantityMeasurementEntity(op, firstDto, targetDto!, result);

            return new QuantityMeasurementEntity(op, firstDto, secondDto, result, targetDto);
        }

        private static QuantityDTO BuildDTO(string measurementType, double value, string unit) =>
            measurementType switch
            {
                "Length"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.LengthUnit>(unit, true)),
                "Weight"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.WeightUnit>(unit, true)),
                "Volume"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.VolumeUnit>(unit, true)),
                "Temperature" => new QuantityDTO(value, Enum.Parse<QuantityDTO.TemperatureUnit>(unit, true)),
                _ => throw new DatabaseException($"Unknown measurement type in DB: {measurementType}")
            };
    }
}