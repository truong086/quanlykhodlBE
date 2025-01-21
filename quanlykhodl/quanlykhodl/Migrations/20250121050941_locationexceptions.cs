using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class locationexceptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "max",
                table: "areas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "locationExceptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_area = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: true),
                    max = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CretorEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locationExceptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_locationExceptions_areas_id_area",
                        column: x => x.id_area,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_locationExceptions_id_area",
                table: "locationExceptions",
                column: "id_area");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locationExceptions");

            migrationBuilder.DropColumn(
                name: "max",
                table: "areas");
        }
    }
}
