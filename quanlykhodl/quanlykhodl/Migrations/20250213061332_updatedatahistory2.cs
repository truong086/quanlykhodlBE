using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatedatahistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "plan_id",
                table: "producthisstorylocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "product_old",
                table: "producthisstorylocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_producthisstorylocations_plan_id",
                table: "producthisstorylocations",
                column: "plan_id");

            migrationBuilder.AddForeignKey(
                name: "FK_producthisstorylocations_plans_plan_id",
                table: "producthisstorylocations",
                column: "plan_id",
                principalTable: "plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_producthisstorylocations_plans_plan_id",
                table: "producthisstorylocations");

            migrationBuilder.DropIndex(
                name: "IX_producthisstorylocations_plan_id",
                table: "producthisstorylocations");

            migrationBuilder.DropColumn(
                name: "plan_id",
                table: "producthisstorylocations");

            migrationBuilder.DropColumn(
                name: "product_old",
                table: "producthisstorylocations");
        }
    }
}
