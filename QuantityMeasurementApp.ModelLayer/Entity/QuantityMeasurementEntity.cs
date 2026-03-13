// ============================================================
// PROJECT : QuantityMeasurementApp.ModelLayer
// FILE    : QuantityMeasurementEntity.cs
//
// UC-15 : N-Tier Architecture
//
// Purpose : Immutable entity stored in the Repository layer to
//           record every operation (compare/convert/add/subtract/
//           divide) along with its operands, result, and any error.
//
// NOTE    : PURELY ADDITIVE – no existing code is modified.
// ============================================================

using System;
using System.Text;

namespace QuantityMeasurementApp.ModelLayer
{
    [Serializable]
    public class QuantityMeasurementEntity
    {
        // ── Operation type ────────────────────────────────────────────────────────────

        public enum OperationType { Compare, Convert, Add, Subtract, Divide }

        // ── Properties ───────────────────────────────────────────────────────────────

        public Guid          Id            { get; }
        public DateTime      Timestamp     { get; }
        public OperationType Operation     { get; }

        public QuantityDTO   FirstOperand  { get; }
        public QuantityDTO?  SecondOperand { get; }   // null for single-operand (Convert)
        public QuantityDTO?  TargetUnit    { get; }   // populated for "with target" ops

        public string        ResultDisplay { get; }
        public bool          HasError      { get; }
        public string?       ErrorMessage  { get; }

        // ── Constructor : single-operand (Convert) ────────────────────────────────────

        public QuantityMeasurementEntity(
            OperationType op, QuantityDTO first, QuantityDTO targetUnit, string result)
        {
            Id            = Guid.NewGuid();
            Timestamp     = DateTime.UtcNow;
            Operation     = op;
            FirstOperand  = first      ?? throw new ArgumentNullException(nameof(first));
            TargetUnit    = targetUnit ?? throw new ArgumentNullException(nameof(targetUnit));
            ResultDisplay = result ?? "";
            HasError      = false;
        }

        // ── Constructor : binary operation ────────────────────────────────────────────

        public QuantityMeasurementEntity(
            OperationType op, QuantityDTO first, QuantityDTO second,
            string result, QuantityDTO? targetUnit = null)
        {
            Id            = Guid.NewGuid();
            Timestamp     = DateTime.UtcNow;
            Operation     = op;
            FirstOperand  = first  ?? throw new ArgumentNullException(nameof(first));
            SecondOperand = second ?? throw new ArgumentNullException(nameof(second));
            TargetUnit    = targetUnit;
            ResultDisplay = result ?? "";
            HasError      = false;
        }

        // ── Constructor : error ───────────────────────────────────────────────────────

        public QuantityMeasurementEntity(
            OperationType op, QuantityDTO first, QuantityDTO? second, string errorMessage,
            bool isError /* disambiguate from binary ctor */)
        {
            Id            = Guid.NewGuid();
            Timestamp     = DateTime.UtcNow;
            Operation     = op;
            FirstOperand  = first ?? throw new ArgumentNullException(nameof(first));
            SecondOperand = second;
            ResultDisplay = "ERROR";
            HasError      = true;
            ErrorMessage  = errorMessage ?? "Unknown error";
        }

        // ── ToString ──────────────────────────────────────────────────────────────────

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Operation}");
            sb.Append($" | Op1: {FirstOperand}");
            if (SecondOperand is not null) sb.Append($" | Op2: {SecondOperand}");
            if (TargetUnit    is not null) sb.Append($" | Target: {TargetUnit.UnitLabel}");
            sb.Append(HasError ? $" | ERROR: {ErrorMessage}" : $" | Result: {ResultDisplay}");
            return sb.ToString();
        }
    }
}
