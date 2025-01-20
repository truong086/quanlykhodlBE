using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatetax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tax",
                table: "importforms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPercentage",
                table: "importforms",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tax",
                table: "importforms");

            migrationBuilder.DropColumn(
                name: "isPercentage",
                table: "importforms");
        }
    }
}
