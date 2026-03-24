// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Interface/IQuantityMeasurementRepository.cs
// UC-17   : Repository contract — separated into its own file.
//
// WHY THIS FILE EXISTS:
//   Defines the contract that ALL repository implementations
//   must follow (Cache, DB, Redis). Controllers and services
//   depend on this interface, not concrete classes — DIP.
//   Equivalent to Spring Data JPA's Repository interface.
// ============================================================

using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.RepositoryLayer.Interface
{
    public interface IQuantityMeasurementRepository
    {
        // ── Write ─────────────────────────────────────────────────────────────────
        void Save(QuantityMeasurementEntity entity);

        // ── Read ──────────────────────────────────────────────────────────────────
        QuantityMeasurementEntity?                    FindById(Guid id);
        IReadOnlyList<QuantityMeasurementEntity>      FindAll();
        IReadOnlyList<QuantityMeasurementEntity>      FindByOperation(QuantityMeasurementEntity.OperationType op);
        IReadOnlyList<QuantityMeasurementEntity>      FindByMeasurementType(string measurementType);

        // ── Statistics ────────────────────────────────────────────────────────────
        int GetTotalCount();
        int GetCountByOperation(QuantityMeasurementEntity.OperationType op);
        int GetErrorCount();

        // ── Delete ────────────────────────────────────────────────────────────────
        void Clear();

        // ── Cleanup ───────────────────────────────────────────────────────────────
        void ReleaseResources();
    }
}