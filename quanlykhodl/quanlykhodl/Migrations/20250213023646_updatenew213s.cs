using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatenew213s : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shelfs_areas_area",
                table: "shelfs");

            migrationBuilder.DropIndex(
                name: "IX_shelfs_area",
                table: "shelfs");

            migrationBuilder.DropColumn(
                name: "area",
                table: "shelfs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "area",
                table: "shelfs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_shelfs_area",
                table: "shelfs",
                column: "area");

            migrationBuilder.AddForeignKey(
                name: "FK_shelfs_areas_area",
                table: "shelfs",
                column: "area",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
