using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updateplanlocationproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_plans_products1_product_map",
                table: "plans");

            migrationBuilder.RenameColumn(
                name: "product_map",
                table: "plans",
                newName: "productlocation_map");

            migrationBuilder.RenameIndex(
                name: "IX_plans_product_map",
                table: "plans",
                newName: "IX_plans_productlocation_map");

            migrationBuilder.AddForeignKey(
                name: "FK_plans_productlocations_productlocation_map",
                table: "plans",
                column: "productlocation_map",
                principalTable: "productlocations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_plans_productlocations_productlocation_map",
                table: "plans");

            migrationBuilder.RenameColumn(
                name: "productlocation_map",
                table: "plans",
                newName: "product_map");

            migrationBuilder.RenameIndex(
                name: "IX_plans_productlocation_map",
                table: "plans",
                newName: "IX_plans_product_map");

            migrationBuilder.AddForeignKey(
                name: "FK_plans_products1_product_map",
                table: "plans",
                column: "product_map",
                principalTable: "products1",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
