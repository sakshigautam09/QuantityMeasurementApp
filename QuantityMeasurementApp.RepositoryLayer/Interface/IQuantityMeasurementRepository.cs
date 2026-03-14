// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : IQuantityMeasurementRepository.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Defines the data-access contract for persisting
//           QuantityMeasurementEntity records.  Follows ISP –
//           only the operations actually needed are declared.
//
// Concrete implementations:
//   • QuantityMeasurementCacheRepository  (in-memory + disk)
//   Future: DatabaseRepository, CloudRepository …
//
// NOTE : PURELY ADDITIVE – no existing code is modified.
// ============================================================

using System;
using System.Collections.Generic;
using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.RepositoryLayer
{
    public interface IQuantityMeasurementRepository
    {
        /// <summary>Persist a new measurement entity.</summary>
        void Save(QuantityMeasurementEntity entity);

        /// <summary>Find a single entity by its unique id.</summary>
        QuantityMeasurementEntity? FindById(Guid id);

        /// <summary>Return all persisted entities (read-only view).</summary>
        IReadOnlyList<QuantityMeasurementEntity> FindAll();

        /// <summary>Remove all persisted entities (reset).</summary>
        void Clear();
    }
}