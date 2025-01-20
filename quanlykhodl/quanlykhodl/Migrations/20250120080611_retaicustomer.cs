using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class retaicustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Retailcustomers_deliverynotes_deliverynote",
                table: "Retailcustomers");

            migrationBuilder.DropIndex(
                name: "IX_Retailcustomers_deliverynote",
                table: "Retailcustomers");

            migrationBuilder.DropColumn(
                name: "deliverynote",
                table: "Retailcustomers");

            migrationBuilder.AddColumn<int>(
                name: "retailcustomers",
                table: "deliverynotes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotes_retailcustomers",
                table: "deliverynotes",
                column: "retailcustomers");

            migrationBuilder.AddForeignKey(
                name: "FK_deliverynotes_Retailcustomers_retailcustomers",
                table: "deliverynotes",
                column: "retailcustomers",
                principalTable: "Retailcustomers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deliverynotes_Retailcustomers_retailcustomers",
                table: "deliverynotes");

            migrationBuilder.DropIndex(
                name: "IX_deliverynotes_retailcustomers",
                table: "deliverynotes");

            migrationBuilder.DropColumn(
                name: "retailcustomers",
                table: "deliverynotes");

            migrationBuilder.AddColumn<int>(
                name: "deliverynote",
                table: "Retailcustomers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Retailcustomers_deliverynote",
                table: "Retailcustomers",
                column: "deliverynote");

            migrationBuilder.AddForeignKey(
                name: "FK_Retailcustomers_deliverynotes_deliverynote",
                table: "Retailcustomers",
                column: "deliverynote",
                principalTable: "deliverynotes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
