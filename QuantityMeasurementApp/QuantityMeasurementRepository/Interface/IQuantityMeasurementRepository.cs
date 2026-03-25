namespace QuantityMeasurementRepository
{
    /// <summary>
    /// UC16: Extended repository interface.
    /// Adds GetByOperation, GetByCategory, GetTotalCount on top of
    /// the UC15 Save / GetAllMeasurements / Clear contract.
    /// Default methods for pool statistics and resource release let
    /// the existing QuantityMeasurementCacheRepository compile unchanged.
    /// </summary>
    public interface IQuantityMeasurementRepository
    {
        // ── UC15 contract (unchanged) ─────────────────────────────────────
        void Save(QuantityMeasurementEntity entity);
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
        void Clear();

        // ── UC16 extensions ───────────────────────────────────────────────

        /// <summary>Filter by operation type: CONVERT, COMPARE, ADD, SUBTRACT, DIVIDE.</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operation)
            => GetAllMeasurements()
               .Where(e => e.OperationType.Equals(operation, StringComparison.OrdinalIgnoreCase))
               .ToList();

        /// <summary>Filter by measurement category: LENGTH, WEIGHT, VOLUME, TEMPERATURE.</summary>
        IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
            => GetAllMeasurements()
               .Where(e =>
                   (e.Operand1?.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (e.Operand2?.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false))
               .ToList();

        /// <summary>Total number of stored measurements.</summary>
        int GetTotalCount() => GetAllMeasurements().Count;

        /// <summary>Connection-pool statistics (no-op for in-memory repo).</summary>
        string GetPoolStatistics()
            => "[Repository] Pool statistics not available for this implementation.";

        /// <summary>Release any held resources (no-op for in-memory repo).</summary>
        void ReleaseResources() { }
    }
}
