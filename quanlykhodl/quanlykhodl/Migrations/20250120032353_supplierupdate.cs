using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class supplierupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_importforms_suppliers_supplier",
                table: "importforms");

            migrationBuilder.DropIndex(
                name: "IX_importforms_supplier",
                table: "importforms");

            migrationBuilder.DropColumn(
                name: "supplier",
                table: "importforms");

            migrationBuilder.AddColumn<int>(
                name: "supplier",
                table: "productImportforms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_productImportforms_supplier",
                table: "productImportforms",
                column: "supplier");

            migrationBuilder.AddForeignKey(
                name: "FK_productImportforms_suppliers_supplier",
                table: "productImportforms",
                column: "supplier",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productImportforms_suppliers_supplier",
                table: "productImportforms");

            migrationBuilder.DropIndex(
                name: "IX_productImportforms_supplier",
                table: "productImportforms");

            migrationBuilder.DropColumn(
                name: "supplier",
                table: "productImportforms");

            migrationBuilder.AddColumn<int>(
                name: "supplier",
                table: "importforms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_importforms_supplier",
                table: "importforms",
                column: "supplier");

            migrationBuilder.AddForeignKey(
                name: "FK_importforms_suppliers_supplier",
                table: "importforms",
                column: "supplier",
                principalTable: "suppliers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
