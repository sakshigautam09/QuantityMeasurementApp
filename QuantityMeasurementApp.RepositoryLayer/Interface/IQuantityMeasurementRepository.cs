// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : IQuantityMeasurementRepository.cs
// UC-16   : Database Integration
//
// Enhanced from UC-15 — adds query methods needed by
// menu options 7–11 in the console.
// ============================================================

using System;
using System.Collections.Generic;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public interface IQuantityMeasurementRepository
    {
        // ── Basic CRUD ────────────────────────────────────────────────────────────────

        void Save(QuantityMeasurementEntity entity);

        QuantityMeasurementEntity? FindById(Guid id);

        // Menu option 7 — View All History
        IReadOnlyList<QuantityMeasurementEntity> FindAll();

        // Menu option 8 — View By Operation Type
        IReadOnlyList<QuantityMeasurementEntity> FindByOperation(
            QuantityMeasurementEntity.OperationType operation);

        // Menu option 9 — View By Measurement Type
        IReadOnlyList<QuantityMeasurementEntity> FindByMeasurementType(string measurementType);

        // Menu option 10 — View Statistics
        int GetTotalCount();
        int GetCountByOperation(QuantityMeasurementEntity.OperationType operation);
        int GetErrorCount();

        // Menu option 11 — Clear All Records
        void Clear();

        // Resource cleanup
        void ReleaseResources();
    }
}