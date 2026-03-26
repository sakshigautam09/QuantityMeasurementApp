using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Context
{
    /// <summary>
    /// EF Core DbContext targeting SQL Server (SSMS).
    /// Auto-migrates on startup via db.Database.MigrateAsync() in Program.cs (WebAPI).
    /// Connection string: QuantityMeasurementWebAPI/Config/appsettings.json
    /// Manual migration: dotnet ef database update --project QuantityMeasurementRepository --startup-project QuantityMeasurementWebAPI
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserEntity>                  Users        { get; set; } = null!;
        public DbSet<QuantityMeasurementEFEntity> Measurements { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("users");
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<QuantityMeasurementEFEntity>(entity =>
            {
                entity.ToTable("quantity_measurements");
                entity.HasIndex(m => m.Operation).HasDatabaseName("IX_qm_operation");
                entity.HasIndex(m => m.Operand1Category).HasDatabaseName("IX_qm_category");
                entity.HasOne(m => m.User)
                      .WithMany(u => u.Measurements)
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });
        }
    }
}
