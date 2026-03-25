using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementModel;
using QuantityMeasurementRepository.Exceptions;

namespace QuantityMeasurementRepository.Database
{
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly string _connectionString;

        public QuantityMeasurementDatabaseRepository()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            _connectionString = config.GetConnectionString("QuantityMeasurementDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'QuantityMeasurementDb' not found in appsettings.json.");

            Console.WriteLine("[DatabaseRepository] Connection string loaded.");
        }

        // ── Save ─────────────────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("sp_SaveMeasurement", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@operand1_value",
                    entity.Operand1 is not null ? (object)entity.Operand1.Value    : DBNull.Value);
                cmd.Parameters.AddWithValue("@operand1_unit",
                    entity.Operand1 is not null ? (object)entity.Operand1.UnitName : DBNull.Value);
                cmd.Parameters.AddWithValue("@operand1_category",
                    entity.Operand1 is not null ? (object)entity.Operand1.Category : DBNull.Value);

                cmd.Parameters.AddWithValue("@operand2_value",
                    entity.Operand2 is not null ? (object)entity.Operand2.Value    : DBNull.Value);
                cmd.Parameters.AddWithValue("@operand2_unit",
                    entity.Operand2 is not null ? (object)entity.Operand2.UnitName : DBNull.Value);
                cmd.Parameters.AddWithValue("@operand2_category",
                    entity.Operand2 is not null ? (object)entity.Operand2.Category : DBNull.Value);

                cmd.Parameters.AddWithValue("@result_value",
                    entity.Result is not null ? (object)entity.Result.Value    : DBNull.Value);
                cmd.Parameters.AddWithValue("@result_unit",
                    entity.Result is not null ? (object)entity.Result.UnitName : DBNull.Value);
                cmd.Parameters.AddWithValue("@result_category",
                    entity.Result is not null ? (object)entity.Result.Category : DBNull.Value);

                cmd.Parameters.AddWithValue("@operation",    entity.OperationType);
                cmd.Parameters.AddWithValue("@timestamp",    entity.Timestamp);
                cmd.Parameters.AddWithValue("@has_error",    entity.HasError ? 1 : 0);
                cmd.Parameters.AddWithValue("@error_message",
                    string.IsNullOrEmpty(entity.ErrorMessage)
                        ? DBNull.Value
                        : (object)entity.ErrorMessage);

                cmd.ExecuteNonQuery();

                AppendHistory(connection, entity.OperationType, "SAVE");

                Console.WriteLine($"[DatabaseRepository] Saved: {entity.OperationType}");
            }
            catch (Exception ex) when (ex is not DatabaseException)
            {
                throw new DatabaseException("Failed to save measurement.", ex);
            }
        }

        // ── GetAllMeasurements ────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetAllMeasurements", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                return ReadEntities(cmd);
            }
            catch (Exception ex) when (ex is not DatabaseException)
            {
                throw new DatabaseException("Failed to retrieve all measurements.", ex);
            }
        }

        // ── GetByOperation ────────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetMeasurementsByOperation", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@operation", operation.ToUpperInvariant());
                return ReadEntities(cmd);
            }
            catch (Exception ex) when (ex is not DatabaseException)
            {
                throw new DatabaseException($"Failed to query by operation '{operation}'.", ex);
            }
        }

        // ── GetByCategory ─────────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetMeasurementsByCategory", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@category", category.ToUpperInvariant());
                return ReadEntities(cmd);
            }
            catch (Exception ex) when (ex is not DatabaseException)
            {
                throw new DatabaseException($"Failed to query by category '{category}'.", ex);
            }
        }

        // ── GetTotalCount ─────────────────────────────────────────────────

        public int GetTotalCount()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetTotalMeasurementCount", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                object? scalar = cmd.ExecuteScalar();
                return scalar is null or DBNull ? 0 : Convert.ToInt32(scalar);
            }
            catch (Exception ex) when (ex is not DatabaseException)
            {
                throw new DatabaseException("Failed to retrieve total count.", ex);
            }
        }

        // ── Clear ─────────────────────────────────────────────────────────

        public void Clear()
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            try
            {
                using SqlCommand cmd = new SqlCommand("sp_DeleteAllMeasurements", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                Console.WriteLine("[DatabaseRepository] All measurements deleted.");
            }
            catch (Exception ex) when (ex is not DatabaseException)
            {
                throw new DatabaseException("Failed to delete all measurements.", ex);
            }
        }

        // ── Private Helpers ───────────────────────────────────────────────

        private static List<QuantityMeasurementEntity> ReadEntities(SqlCommand cmd)
        {
            List<QuantityMeasurementEntity> list = new();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapRow(reader));
            return list;
        }

        private static QuantityMeasurementEntity MapRow(SqlDataReader reader)
        {
            string operation = reader["operation"].ToString()!;
            bool   hasError  = Convert.ToBoolean(reader["has_error"]);
            string errorMsg  = reader["error_message"] == DBNull.Value
                                   ? string.Empty
                                   : reader["error_message"].ToString()!;

            QuantityDTO? op1    = ReadDTO(reader, "operand1_value", "operand1_unit", "operand1_category");
            QuantityDTO? op2    = ReadDTO(reader, "operand2_value", "operand2_unit", "operand2_category");
            QuantityDTO? result = ReadDTO(reader, "result_value",   "result_unit",   "result_category");

            if (hasError)
                return new QuantityMeasurementEntity(operation, op1, op2, errorMsg);

            if (op2 is not null && result is not null)
                return new QuantityMeasurementEntity(operation, op1!, op2, result);

            return new QuantityMeasurementEntity(operation, op1!, result);
        }

        private static QuantityDTO? ReadDTO(
            SqlDataReader reader, string valueCol, string unitCol, string categoryCol)
        {
            if (reader[valueCol] == DBNull.Value) return null;
            return new QuantityDTO(
                Convert.ToDouble(reader[valueCol]),
                reader[unitCol].ToString()!,
                reader[categoryCol].ToString()!);
        }

        private static void AppendHistory(SqlConnection connection, string operationType, string action)
        {
            const string sql =
                "INSERT INTO quantity_measurement_history " +
                "  (operation_type, action, action_timestamp) " +
                "VALUES (@operation_type, @action, @action_timestamp)";

            using SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@operation_type",   operationType);
            cmd.Parameters.AddWithValue("@action",           action);
            cmd.Parameters.AddWithValue("@action_timestamp", DateTime.UtcNow);
            cmd.ExecuteNonQuery();
        }
    }
}