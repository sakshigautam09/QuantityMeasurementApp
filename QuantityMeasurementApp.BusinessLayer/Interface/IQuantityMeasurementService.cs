// ============================================================
// PROJECT : QuantityMeasurementApp.BusinessLayer
// FILE    : IQuantityMeasurementService.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Service contract that accepts/returns QuantityDTO
//           objects.  The controller never touches Core entities.
//
// All 5 operations from UC1-UC14 are exposed here:
//   Compare / Convert / Add / Subtract / Divide
//   (each with and without an explicit target-unit overload)
//
// NOTE : PURELY ADDITIVE – no existing code is modified.
// ============================================================

using QuantityMeasurementApp.ModelLayer;

namespace QuantityMeasurementApp.BusinessLayer
{
    public interface IQuantityMeasurementService
    {
        // ── Compare ──────────────────────────────────────────────────────────────────
        /// <summary>Returns DTO with Value=1 (equal) or Value=0 (not equal).</summary>
        QuantityDTO Compare(QuantityDTO first, QuantityDTO second);

        // ── Convert ──────────────────────────────────────────────────────────────────
        /// <summary>Convert source to the unit described by targetUnit DTO.</summary>
        QuantityDTO Convert(QuantityDTO source, QuantityDTO targetUnit);

        // ── Add ───────────────────────────────────────────────────────────────────────
        /// <summary>Add two quantities; result unit = unit of first.</summary>
        QuantityDTO Add(QuantityDTO first, QuantityDTO second);

        /// <summary>Add two quantities; result expressed in targetUnit.</summary>
        QuantityDTO Add(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit);

        // ── Subtract ─────────────────────────────────────────────────────────────────
        /// <summary>Subtract second from first; result unit = unit of first.</summary>
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);

        /// <summary>Subtract and express result in targetUnit.</summary>
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second, QuantityDTO targetUnit);

        // ── Divide ───────────────────────────────────────────────────────────────────
        /// <summary>Divide first by second; returns dimensionless scalar in a DTO.</summary>
        QuantityDTO Divide(QuantityDTO first, QuantityDTO second);
    }
}
