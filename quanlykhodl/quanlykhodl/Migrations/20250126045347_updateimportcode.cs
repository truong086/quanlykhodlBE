using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updateimportcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAction",
                table: "productlocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "productImportforms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAction",
                table: "productImportforms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActualQuantity",
                table: "importforms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAction",
                table: "importforms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAction",
                table: "productlocations");

            migrationBuilder.DropColumn(
                name: "code",
                table: "productImportforms");

            migrationBuilder.DropColumn(
                name: "isAction",
                table: "productImportforms");

            migrationBuilder.DropColumn(
                name: "ActualQuantity",
                table: "importforms");

            migrationBuilder.DropColumn(
                name: "isAction",
                table: "importforms");
        }
    }
}
