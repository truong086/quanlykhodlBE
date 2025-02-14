using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatelinedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lines_areas_id_area",
                table: "lines");

            migrationBuilder.DropForeignKey(
                name: "FK_shelfs_lines_line",
                table: "shelfs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lines",
                table: "lines");

            migrationBuilder.RenameTable(
                name: "lines",
                newName: "linespage");

            migrationBuilder.RenameIndex(
                name: "IX_lines_id_area",
                table: "linespage",
                newName: "IX_linespage_id_area");

            migrationBuilder.AddPrimaryKey(
                name: "PK_linespage",
                table: "linespage",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_linespage_areas_id_area",
                table: "linespage",
                column: "id_area",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shelfs_linespage_line",
                table: "shelfs",
                column: "line",
                principalTable: "linespage",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_linespage_areas_id_area",
                table: "linespage");

            migrationBuilder.DropForeignKey(
                name: "FK_shelfs_linespage_line",
                table: "shelfs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_linespage",
                table: "linespage");

            migrationBuilder.RenameTable(
                name: "linespage",
                newName: "lines");

            migrationBuilder.RenameIndex(
                name: "IX_linespage_id_area",
                table: "lines",
                newName: "IX_lines_id_area");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lines",
                table: "lines",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_lines_areas_id_area",
                table: "lines",
                column: "id_area",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shelfs_lines_line",
                table: "shelfs",
                column: "line",
                principalTable: "lines",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
