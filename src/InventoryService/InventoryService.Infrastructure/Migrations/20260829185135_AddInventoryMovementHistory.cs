using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMovementHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inventory_movements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    movement_type = table.Column<int>(type: "integer", nullable: false),
                    from_department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    to_department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    from_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    to_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_movements", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_inventory_movements_inventory_id",
                table: "inventory_movements",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_movements_inventory_occurred",
                table: "inventory_movements",
                columns: new[] { "inventory_id", "occurred_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_movements");
        }
    }
}
