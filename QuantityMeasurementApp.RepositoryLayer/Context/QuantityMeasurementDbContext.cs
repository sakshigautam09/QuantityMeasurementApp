// ============================================================
// PROJECT : QuantityMeasurementApp.RepositoryLayer
// FILE    : Context/QuantityMeasurementDbContext.cs
// UC-17   : EF Core DbContext — ORM bridge, replaces ADO.NET
// ============================================================
using Microsoft.EntityFrameworkCore;
namespace QuantityMeasurementApp.RepositoryLayer.Context
{
    public class QuantityMeasurementDbContext : DbContext
    {
        public QuantityMeasurementDbContext(DbContextOptions<QuantityMeasurementDbContext> options) : base(options) { }
        public DbSet<QuantityMeasurementDbEntity> QuantityMeasurements { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<QuantityMeasurementDbEntity>(e => {
                e.ToTable("quantity_measurements");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                e.Property(x => x.Timestamp).HasColumnName("timestamp").IsRequired();
                e.Property(x => x.Operation).HasColumnName("operation").IsRequired().HasMaxLength(20);
                e.Property(x => x.MeasurementType).HasColumnName("measurement_type").IsRequired().HasMaxLength(50);
                e.Property(x => x.FirstValue).HasColumnName("first_value").IsRequired();
                e.Property(x => x.FirstUnit).HasColumnName("first_unit").IsRequired().HasMaxLength(50);
                e.Property(x => x.SecondValue).HasColumnName("second_value").IsRequired(false);
                e.Property(x => x.SecondUnit).HasColumnName("second_unit").IsRequired(false).HasMaxLength(50);
                e.Property(x => x.TargetUnit).HasColumnName("target_unit").IsRequired(false).HasMaxLength(50);
                e.Property(x => x.ResultDisplay).HasColumnName("result_display").IsRequired().HasMaxLength(200);
                e.Property(x => x.HasError).HasColumnName("has_error").IsRequired();
                e.Property(x => x.ErrorMessage).HasColumnName("error_message").IsRequired(false).HasMaxLength(500);
                e.HasIndex(x => x.Operation).HasDatabaseName("IX_qm_operation");
                e.HasIndex(x => x.MeasurementType).HasDatabaseName("IX_qm_type");
                e.HasIndex(x => x.Timestamp).HasDatabaseName("IX_qm_timestamp");
            });
            mb.Entity<UserEntity>(e => {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
                e.Property(x => x.Username).HasColumnName("username").IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).HasColumnName("email").IsRequired().HasMaxLength(200);
                e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired().HasMaxLength(500);
                e.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
                e.Property(x => x.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
                e.HasIndex(x => x.Username).IsUnique().HasDatabaseName("UQ_users_username");
                e.HasIndex(x => x.Email).IsUnique().HasDatabaseName("UQ_users_email");
            });
        }
    }
}