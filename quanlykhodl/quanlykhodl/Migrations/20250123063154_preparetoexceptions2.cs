using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class preparetoexceptions2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_imageProducts_products1_products_idid",
                table: "imageProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_productlocations_products1_productsid",
                table: "productlocations");

            migrationBuilder.DropIndex(
                name: "IX_productlocations_productsid",
                table: "productlocations");

            migrationBuilder.DropIndex(
                name: "IX_imageProducts_products_idid",
                table: "imageProducts");

            migrationBuilder.DropColumn(
                name: "productsid",
                table: "productlocations");

            migrationBuilder.DropColumn(
                name: "products_idid",
                table: "imageProducts");

            migrationBuilder.AddColumn<int>(
                name: "account_id",
                table: "prepareToExports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations",
                column: "id_product");

            migrationBuilder.CreateIndex(
                name: "IX_prepareToExports_account_id",
                table: "prepareToExports",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_imageProducts_productMap",
                table: "imageProducts",
                column: "productMap");

            migrationBuilder.AddForeignKey(
                name: "FK_imageProducts_products1_productMap",
                table: "imageProducts",
                column: "productMap",
                principalTable: "products1",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prepareToExports_accounts_account_id",
                table: "prepareToExports",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_imageProducts_products1_productMap",
                table: "imageProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_prepareToExports_accounts_account_id",
                table: "prepareToExports");

            migrationBuilder.DropForeignKey(
                name: "FK_productlocations_products1_id_product",
                table: "productlocations");

            migrationBuilder.DropIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations");

            migrationBuilder.DropIndex(
                name: "IX_prepareToExports_account_id",
                table: "prepareToExports");

            migrationBuilder.DropIndex(
                name: "IX_imageProducts_productMap",
                table: "imageProducts");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "prepareToExports");

            migrationBuilder.AddColumn<int>(
                name: "productsid",
                table: "productlocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "products_idid",
                table: "imageProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_productsid",
                table: "productlocations",
                column: "productsid");

            migrationBuilder.CreateIndex(
                name: "IX_imageProducts_products_idid",
                table: "imageProducts",
                column: "products_idid");

            migrationBuilder.AddForeignKey(
                name: "FK_imageProducts_products1_products_idid",
                table: "imageProducts",
                column: "products_idid",
                principalTable: "products1",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_productlocations_products1_productsid",
                table: "productlocations",
                column: "productsid",
                principalTable: "products1",
                principalColumn: "id");
        }
    }
}
