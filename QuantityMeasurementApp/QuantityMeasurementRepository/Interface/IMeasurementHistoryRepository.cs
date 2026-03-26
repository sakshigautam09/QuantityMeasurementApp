namespace QuantityMeasurementRepository
{
    /// <summary>
    /// Sync repository interface for measurement history.
    /// Adds GetByOperation, GetByCategory, GetTotalCount on top of
    /// Save / GetAllMeasurements / Clear contract.
    /// </summary>
    public interface IMeasurementHistoryRepository
    {
        void Save(QuantityMeasurementEntity entity);
        IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
        void Clear();

        IReadOnlyList<QuantityMeasurementEntity> GetByOperation(string operation)
            => GetAllMeasurements()
               .Where(e => e.OperationType.Equals(operation, StringComparison.OrdinalIgnoreCase))
               .ToList();

        IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
            => GetAllMeasurements()
               .Where(e =>
                   (e.Operand1?.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   (e.Operand2?.Category.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false))
               .ToList();

        int GetTotalCount() => GetAllMeasurements().Count;

        string GetPoolStatistics()
            => "[Repository] Pool statistics not available for this implementation.";

        void ReleaseResources() { }
    }
}
