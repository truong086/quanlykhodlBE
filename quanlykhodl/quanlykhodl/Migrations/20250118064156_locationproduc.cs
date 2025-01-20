using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class locationproduc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productlocation_products1_productsid",
                table: "productlocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_productlocation",
                table: "productlocation");

            migrationBuilder.DropIndex(
                name: "IX_productlocation_productsid",
                table: "productlocation");

            migrationBuilder.DropColumn(
                name: "productsid",
                table: "productlocation");

            migrationBuilder.RenameTable(
                name: "productlocation",
                newName: "productlocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_productlocations",
                table: "productlocations",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations",
                column: "id_product");

            migrationBuilder.AddForeignKey(
                name: "FK_productlocations_products1_id_product",
                table: "productlocations",
                column: "id_product",
                principalTable: "products1",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productlocations_products1_id_product",
                table: "productlocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_productlocations",
                table: "productlocations");

            migrationBuilder.DropIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations");

            migrationBuilder.RenameTable(
                name: "productlocations",
                newName: "productlocation");

            migrationBuilder.AddColumn<int>(
                name: "productsid",
                table: "productlocation",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_productlocation",
                table: "productlocation",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_productlocation_productsid",
                table: "productlocation",
                column: "productsid");

            migrationBuilder.AddForeignKey(
                name: "FK_productlocation_products1_productsid",
                table: "productlocation",
                column: "productsid",
                principalTable: "products1",
                principalColumn: "id");
        }
    }
}
