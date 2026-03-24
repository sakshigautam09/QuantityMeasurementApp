// ============================================================
// PROJECT : QuantityMeasurementApp.ModelLayer
// FILE    : Entity/QuantityMeasurementEntity.cs
// UC-15   : N-Tier Architecture
// UC-17   : Added validation annotations for API layer.
// ============================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuantityMeasurementApp.ModelLayer
{
    [Serializable]
    public class QuantityMeasurementEntity
    {
        // ── Operation type ────────────────────────────────────────────────────────

        public enum OperationType { Compare, Convert, Add, Subtract, Divide }

        // ── Properties with annotations ──────────────────────────────────────────

        [Required]
        public Guid Id { get; }

        [Required]
        public DateTime Timestamp { get; }

        [Required]
        [EnumDataType(typeof(OperationType),
            ErrorMessage = "Operation must be: Compare, Convert, Add, Subtract or Divide.")]
        public OperationType Operation { get; }

        [Required(ErrorMessage = "First operand is required.")]
        public QuantityDTO FirstOperand { get; }

        // Null for Convert (single operand)
        public QuantityDTO? SecondOperand { get; }

        // Null for Compare and Divide
        public QuantityDTO? TargetUnit { get; }

        [Required(ErrorMessage = "Result display is required.")]
        [MaxLength(200, ErrorMessage = "Result display cannot exceed 200 characters.")]
        public string ResultDisplay { get; }

        [Required]
        public bool HasError { get; }

        [MaxLength(500, ErrorMessage = "Error message cannot exceed 500 characters.")]
        public string? ErrorMessage { get; }

        // ── Constructor : single-operand (Convert) ────────────────────────────────

        public QuantityMeasurementEntity(
            OperationType op,
            QuantityDTO   first,
            QuantityDTO   targetUnit,
            string        result)
        {
            Id            = Guid.NewGuid();
            Timestamp     = DateTime.UtcNow;
            Operation     = op;
            FirstOperand  = first      ?? throw new ArgumentNullException(nameof(first));
            TargetUnit    = targetUnit ?? throw new ArgumentNullException(nameof(targetUnit));
            ResultDisplay = result ?? "";
            HasError      = false;
        }

        // ── Constructor : binary operation ───────────────────────────────────────

        public QuantityMeasurementEntity(
            OperationType op,
            QuantityDTO   first,
            QuantityDTO   second,
            string        result,
            QuantityDTO?  targetUnit = null)
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

        // ── Constructor : error ───────────────────────────────────────────────────

        public QuantityMeasurementEntity(
            OperationType op,
            QuantityDTO   first,
            QuantityDTO?  second,
            string        errorMessage,
            bool          isError)     // disambiguates from binary ctor
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

        // ── ToString ──────────────────────────────────────────────────────────────

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Operation}");
            sb.Append($" | Op1: {FirstOperand}");
            if (SecondOperand is not null) sb.Append($" | Op2: {SecondOperand}");
            if (TargetUnit    is not null) sb.Append($" | Target: {TargetUnit.UnitLabel}");
            sb.Append(HasError
                ? $" | ERROR: {ErrorMessage}"
                : $" | Result: {ResultDisplay}");
            return sb.ToString();
        }
    }
}