using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatetax2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tax",
                table: "deliverynotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPercentage",
                table: "deliverynotes",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tax",
                table: "deliverynotes");

            migrationBuilder.DropColumn(
                name: "isPercentage",
                table: "deliverynotes");
        }
    }
}
