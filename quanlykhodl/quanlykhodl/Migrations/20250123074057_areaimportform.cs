using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class areaimportform : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "area_id",
                table: "productImportforms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location",
                table: "productImportforms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_productImportforms_area_id",
                table: "productImportforms",
                column: "area_id");

            migrationBuilder.AddForeignKey(
                name: "FK_productImportforms_areas_area_id",
                table: "productImportforms",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productImportforms_areas_area_id",
                table: "productImportforms");

            migrationBuilder.DropIndex(
                name: "IX_productImportforms_area_id",
                table: "productImportforms");

            migrationBuilder.DropColumn(
                name: "area_id",
                table: "productImportforms");

            migrationBuilder.DropColumn(
                name: "location",
                table: "productImportforms");
        }
    }
}
