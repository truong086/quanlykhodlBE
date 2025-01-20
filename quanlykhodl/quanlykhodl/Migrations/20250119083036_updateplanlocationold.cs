using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updateplanlocationold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "localtionnew",
                table: "plans",
                newName: "localtionNew");

            migrationBuilder.AddColumn<int>(
                name: "areaOld",
                table: "plans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "floorOld",
                table: "plans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "localtionOld",
                table: "plans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "warehouseOld",
                table: "plans",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "areaOld",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "floorOld",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "localtionOld",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "warehouseOld",
                table: "plans");

            migrationBuilder.RenameColumn(
                name: "localtionNew",
                table: "plans",
                newName: "localtionnew");
        }
    }
}
