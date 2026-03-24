using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementApp.RepositoryLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quantity_measurements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    operation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    measurement_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    first_value = table.Column<double>(type: "float", nullable: false),
                    first_unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    second_value = table.Column<double>(type: "float", nullable: true),
                    second_unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    target_unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    result_display = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    has_error = table.Column<bool>(type: "bit", nullable: false),
                    error_message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quantity_measurements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_qm_operation",
                table: "quantity_measurements",
                column: "operation");

            migrationBuilder.CreateIndex(
                name: "IX_qm_timestamp",
                table: "quantity_measurements",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_qm_type",
                table: "quantity_measurements",
                column: "measurement_type");

            migrationBuilder.CreateIndex(
                name: "UQ_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quantity_measurements");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
