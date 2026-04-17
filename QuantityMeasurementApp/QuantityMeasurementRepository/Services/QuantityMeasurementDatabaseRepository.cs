using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementModel;
using QuantityMeasurementRepository.Context;

namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC16: Sync SQL Server repository for the console app.
    /// Uses EF Core with a fresh DbContext per operation (no DI / no Redis).
    /// Connection string is read from appsettings.json in the app's base directory.
    /// </summary>
    public class QuantityMeasurementDatabaseRepository : IMeasurementHistoryRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public QuantityMeasurementDatabaseRepository()
        {
            // Read connection string from appsettings.json next to the executable
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string connectionString = config.GetConnectionString("QuantityMeasurementDb")
                ?? throw new InvalidOperationException(
                    "Connection string 'QuantityMeasurementDb' not found in appsettings.json");

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            // Ensure the database and tables exist
            using var db = new ApplicationDbContext(_options);
            db.Database.Migrate();

            Console.WriteLine("[DatabaseRepository] Connected to SQL Server successfully.");
        }

        // ── Save ──────────────────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            using var db = new ApplicationDbContext(_options);

            var efEntity = ToEFEntity(entity);
            db.Measurements.Add(efEntity);
            db.SaveChanges();

            Console.WriteLine($"[DatabaseRepository] Saved: {entity.OperationType}");
        }

        // ── GetAllMeasurements ────────────────────────────────────────────

        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            using var db = new ApplicationDbContext(_options);
            return db.Measurements
                     .OrderByDescending(m => m.Timestamp)
                     .AsEnumerable()
                     .Select(FromEFEntity)
                     .ToList();
        }

        // ── Clear ─────────────────────────────────────────────────────────

        public void Clear()
        {
            using var db = new ApplicationDbContext(_options);
            db.Measurements.ExecuteDelete();
            Console.WriteLine("[DatabaseRepository] All measurements deleted from SQL Server.");
        }

        // ── EF Entity Mapping ─────────────────────────────────────────────

        private static QuantityMeasurementModel.Entities.QuantityMeasurementEFEntity ToEFEntity(
            QuantityMeasurementEntity e)
        {
            return new QuantityMeasurementModel.Entities.QuantityMeasurementEFEntity
            {
                Operation        = e.OperationType,
                Timestamp        = e.Timestamp,
                HasError         = e.HasError,
                ErrorMessage     = e.HasError ? e.ErrorMessage : null,

                Operand1Value    = e.Operand1?.Value,
                Operand1Unit     = e.Operand1?.UnitName,
                Operand1Category = e.Operand1?.Category,

                Operand2Value    = e.Operand2?.Value,
                Operand2Unit     = e.Operand2?.UnitName,
                Operand2Category = e.Operand2?.Category,

                ResultValue      = e.Result?.Value,
                ResultUnit       = e.Result?.UnitName,
                ResultCategory   = e.Result?.Category,
            };
        }

        private static QuantityMeasurementEntity FromEFEntity(
            QuantityMeasurementModel.Entities.QuantityMeasurementEFEntity e)
        {
            QuantityDTO? op1 = e.Operand1Value.HasValue
                ? new QuantityDTO(e.Operand1Value.Value, e.Operand1Unit ?? "", e.Operand1Category ?? "")
                : null;

            QuantityDTO? op2 = e.Operand2Value.HasValue
                ? new QuantityDTO(e.Operand2Value.Value, e.Operand2Unit ?? "", e.Operand2Category ?? "")
                : null;

            QuantityDTO? res = e.ResultValue.HasValue
                ? new QuantityDTO(e.ResultValue.Value, e.ResultUnit ?? "", e.ResultCategory ?? "")
                : null;

            if (e.HasError)
                return new QuantityMeasurementEntity(e.Operation, op1, op2, e.ErrorMessage ?? "");

            if (op2 is not null && res is not null)
                return new QuantityMeasurementEntity(e.Operation, op1!, op2, res);

            return new QuantityMeasurementEntity(e.Operation, op1!, res);
        }
    }
}