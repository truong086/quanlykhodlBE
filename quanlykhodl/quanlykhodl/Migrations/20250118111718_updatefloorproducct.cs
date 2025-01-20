using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatefloorproducct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products1_floors_floor_map",
                table: "products1");

            migrationBuilder.DropIndex(
                name: "IX_products1_floor_map",
                table: "products1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_products1_floor_map",
                table: "products1",
                column: "floor_map");

            migrationBuilder.AddForeignKey(
                name: "FK_products1_floors_floor_map",
                table: "products1",
                column: "floor_map",
                principalTable: "floors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
