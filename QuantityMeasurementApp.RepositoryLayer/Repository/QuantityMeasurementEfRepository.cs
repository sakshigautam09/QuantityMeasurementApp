// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Repository/QuantityMeasurementEfRepository.cs
// UC-17   : Pure data access — no encryption here.
//
// WHY NO ENCRYPTION HERE:
//   Repository responsibility = store and retrieve data only.
//   Encryption is a business concern handled in BusinessLayer.
//   QuantityMeasurementServiceImpl encrypts before calling Save()
//   and decrypts after calling FindAll() etc.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuantityMeasurementApp.ModelLayer;
using QuantityMeasurementApp.RepositoryLayer.Context;
using QuantityMeasurementApp.RepositoryLayer.Interface;

namespace QuantityMeasurementApp.RepositoryLayer.Repository
{
    public class QuantityMeasurementEfRepository : IQuantityMeasurementRepository
    {
        private readonly QuantityMeasurementDbContext             _context;
        private readonly ILogger<QuantityMeasurementEfRepository> _logger;

        public QuantityMeasurementEfRepository(
            QuantityMeasurementDbContext             context,
            ILogger<QuantityMeasurementEfRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger  = logger  ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            _logger.LogInformation("Saving: {Op} | {Type}",
                entity.Operation, entity.FirstOperand.Type);
            var dbEntity = MapToDb(entity);
            _context.QuantityMeasurements.Add(dbEntity);
            _context.SaveChanges();
            _logger.LogInformation("Saved Id: {Id}", entity.Id);
        }

        public QuantityMeasurementEntity? FindById(Guid id)
        {
            var dbEntity = _context.QuantityMeasurements
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
            return dbEntity is null ? null : MapFromDb(dbEntity);
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindAll()
        {
            _logger.LogInformation("FindAll.");
            var list = _context.QuantityMeasurements
                .AsNoTracking()
                .OrderByDescending(e => e.Timestamp)
                .AsEnumerable()
                .Select(MapFromDb)
                .ToList();
            _logger.LogInformation("Found {Count} records.", list.Count);
            return list.AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
            QuantityMeasurementEntity.OperationType op)
        {
            string opStr = op.ToString();
            return _context.QuantityMeasurements
                .AsNoTracking()
                .Where(e => e.Operation == opStr)
                .OrderByDescending(e => e.Timestamp)
                .AsEnumerable()
                .Select(MapFromDb)
                .ToList()
                .AsReadOnly();
        }

        public IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(
            string measurementType)
        {
            return _context.QuantityMeasurements
                .AsNoTracking()
                .Where(e => e.MeasurementType == measurementType)
                .OrderByDescending(e => e.Timestamp)
                .AsEnumerable()
                .Select(MapFromDb)
                .ToList()
                .AsReadOnly();
        }

        public int GetTotalCount()
            => _context.QuantityMeasurements.Count();

        public int GetCountByOperation(QuantityMeasurementEntity.OperationType op)
        {
            string opStr = op.ToString();
            return _context.QuantityMeasurements.Count(e => e.Operation == opStr);
        }

        public int GetErrorCount()
            => _context.QuantityMeasurements.Count(e => e.HasError);

        public void Clear()
        {
            _logger.LogWarning("Clearing all records.");
            _context.QuantityMeasurements.ExecuteDelete();
        }

        public void ReleaseResources() { }

        // ── Mapping ───────────────────────────────────────────────────────────────

        private static QuantityMeasurementDbEntity MapToDb(QuantityMeasurementEntity e)
            => new()
            {
                Id              = e.Id,
                Timestamp       = e.Timestamp,
                Operation       = e.Operation.ToString(),
                MeasurementType = e.FirstOperand.Type.ToString(),
                FirstValue      = e.FirstOperand.Value,
                FirstUnit       = e.FirstOperand.UnitLabel,
                SecondValue     = e.SecondOperand?.Value,
                SecondUnit      = e.SecondOperand?.UnitLabel,
                TargetUnit      = e.TargetUnit?.UnitLabel,
                ResultDisplay   = e.ResultDisplay,
                HasError        = e.HasError,
                ErrorMessage    = e.ErrorMessage
            };

        private static QuantityMeasurementEntity MapFromDb(QuantityMeasurementDbEntity r)
        {
            var op    = Enum.Parse<QuantityMeasurementEntity.OperationType>(r.Operation, true);
            var first = BuildDTO(r.MeasurementType, r.FirstValue, r.FirstUnit);

            QuantityDTO? second = r.SecondValue.HasValue && r.SecondUnit is not null
                ? BuildDTO(r.MeasurementType, r.SecondValue.Value, r.SecondUnit)
                : null;

            QuantityDTO? target = r.TargetUnit is not null
                ? BuildDTO(r.MeasurementType, 0.0, r.TargetUnit)
                : null;

            if (r.HasError)
                return new QuantityMeasurementEntity(
                    op, first, second, r.ErrorMessage ?? "Unknown error", true);

            if (second is null)
                return new QuantityMeasurementEntity(op, first, target!, r.ResultDisplay);

            return new QuantityMeasurementEntity(op, first, second, r.ResultDisplay, target);
        }

        private static QuantityDTO BuildDTO(string type, double value, string unit) =>
            type switch
            {
                "Length"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.LengthUnit>(unit, true)),
                "Weight"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.WeightUnit>(unit, true)),
                "Volume"      => new QuantityDTO(value, Enum.Parse<QuantityDTO.VolumeUnit>(unit, true)),
                "Temperature" => new QuantityDTO(value, Enum.Parse<QuantityDTO.TemperatureUnit>(unit, true)),
                _ => throw new InvalidOperationException($"Unknown type: {type}")
            };
    }
}