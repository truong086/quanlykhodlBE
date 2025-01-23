using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updateplan2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Codelocation_areas_id_area",
                table: "Codelocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Codelocation",
                table: "Codelocation");

            migrationBuilder.RenameTable(
                name: "Codelocation",
                newName: "codelocations");

            migrationBuilder.RenameIndex(
                name: "IX_Codelocation_id_area",
                table: "codelocations",
                newName: "IX_codelocations_id_area");

            migrationBuilder.AddPrimaryKey(
                name: "PK_codelocations",
                table: "codelocations",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_codelocations_areas_id_area",
                table: "codelocations",
                column: "id_area",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_codelocations_areas_id_area",
                table: "codelocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_codelocations",
                table: "codelocations");

            migrationBuilder.RenameTable(
                name: "codelocations",
                newName: "Codelocation");

            migrationBuilder.RenameIndex(
                name: "IX_codelocations_id_area",
                table: "Codelocation",
                newName: "IX_Codelocation_id_area");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Codelocation",
                table: "Codelocation",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Codelocation_areas_id_area",
                table: "Codelocation",
                column: "id_area",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
