using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatearea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products1_areas_area_map",
                table: "products1");

            migrationBuilder.DropIndex(
                name: "IX_products1_area_map",
                table: "products1");

            migrationBuilder.DropColumn(
                name: "area_map",
                table: "products1");

            migrationBuilder.AddColumn<int>(
                name: "id_area",
                table: "productlocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_id_area",
                table: "productlocations",
                column: "id_area");

            migrationBuilder.AddForeignKey(
                name: "FK_productlocations_areas_id_area",
                table: "productlocations",
                column: "id_area",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productlocations_areas_id_area",
                table: "productlocations");

            migrationBuilder.DropIndex(
                name: "IX_productlocations_id_area",
                table: "productlocations");

            migrationBuilder.DropColumn(
                name: "id_area",
                table: "productlocations");

            migrationBuilder.AddColumn<int>(
                name: "area_map",
                table: "products1",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_products1_area_map",
                table: "products1",
                column: "area_map");

            migrationBuilder.AddForeignKey(
                name: "FK_products1_areas_area_map",
                table: "products1",
                column: "area_map",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
