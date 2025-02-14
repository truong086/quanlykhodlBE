using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class dataupdatehistorys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "producthisstorylocations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    locationold = table.Column<int>(type: "int", nullable: true),
                    locationnew = table.Column<int>(type: "int", nullable: true),
                    shelfold = table.Column<int>(type: "int", nullable: true),
                    shelfnew = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producthisstorylocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_producthisstorylocations_products1_product_id",
                        column: x => x.product_id,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_producthisstorylocations_product_id",
                table: "producthisstorylocations",
                column: "product_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "producthisstorylocations");
        }
    }
}
