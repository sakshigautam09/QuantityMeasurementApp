using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuantityMeasurementModel.Entities
{
    /// <summary>
    /// UC17: User entity mapped to [users] table in SQL Server (SSMS).
    /// PasswordHash stores BCrypt output: $2a$12$[22-char-salt][31-char-hash] — never plain text.
    /// </summary>
    [Table("users")]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt hash format: $2a$12$[22-char-salt][31-char-hash] (60 chars total).
        /// Salt is embedded — no separate salt column needed.
        /// Work factor 12 = 2^12 = 4096 Blowfish rounds (~250 ms per hash).
        /// </summary>
        [Required]
        [MaxLength(256)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("role")]
        public string Role { get; set; } = "User";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [MaxLength(512)]
        [Column("refresh_token")]
        public string? RefreshToken { get; set; }

        [Column("refresh_token_expiry")]
        public DateTime? RefreshTokenExpiry { get; set; }

        // Navigation property
        public ICollection<QuantityMeasurementEFEntity> Measurements { get; set; }
            = new List<QuantityMeasurementEFEntity>();
    }
}
