using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuantityMeasurementRepository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_PostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    refresh_token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    refresh_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quantity_measurements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    operation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operand1_value = table.Column<double>(type: "double precision", nullable: true),
                    operand1_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    operand1_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    operand2_value = table.Column<double>(type: "double precision", nullable: true),
                    operand2_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    operand2_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    result_value = table.Column<double>(type: "double precision", nullable: true),
                    result_unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    result_category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    has_error = table.Column<bool>(type: "boolean", nullable: false),
                    error_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quantity_measurements", x => x.id);
                    table.ForeignKey(
                        name: "FK_quantity_measurements_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_qm_category",
                table: "quantity_measurements",
                column: "operand1_category");

            migrationBuilder.CreateIndex(
                name: "IX_qm_operation",
                table: "quantity_measurements",
                column: "operation");

            migrationBuilder.CreateIndex(
                name: "IX_quantity_measurements_user_id",
                table: "quantity_measurements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
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
