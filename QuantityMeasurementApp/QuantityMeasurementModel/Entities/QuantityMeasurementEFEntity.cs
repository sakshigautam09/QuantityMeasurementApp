using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC17: EF Core entity mapped to [quantity_measurements] table in SQL Server (SSMS).
    /// Column names match UC16 schema.sql exactly — zero data loss on migration.
    /// </summary>
    [Table("quantity_measurements")]
    public class QuantityMeasurementEFEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("operation")]
        public string Operation { get; set; } = string.Empty;

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // ── Operand 1 ─────────────────────────────────────────────────────
        [Column("operand1_value")]
        public double? Operand1Value { get; set; }

        [MaxLength(50)]
        [Column("operand1_unit")]
        public string? Operand1Unit { get; set; }

        [MaxLength(50)]
        [Column("operand1_category")]
        public string? Operand1Category { get; set; }

        // ── Operand 2 ─────────────────────────────────────────────────────
        [Column("operand2_value")]
        public double? Operand2Value { get; set; }

        [MaxLength(50)]
        [Column("operand2_unit")]
        public string? Operand2Unit { get; set; }

        [MaxLength(50)]
        [Column("operand2_category")]
        public string? Operand2Category { get; set; }

        // ── Result ────────────────────────────────────────────────────────
        [Column("result_value")]
        public double? ResultValue { get; set; }

        [MaxLength(50)]
        [Column("result_unit")]
        public string? ResultUnit { get; set; }

        [MaxLength(50)]
        [Column("result_category")]
        public string? ResultCategory { get; set; }

        // ── Error ─────────────────────────────────────────────────────────
        [Column("has_error")]
        public bool HasError { get; set; } = false;

        [MaxLength(500)]
        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        // ── User FK ───────────────────────────────────────────────────────
        [Column("user_id")]
        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
    }
}
