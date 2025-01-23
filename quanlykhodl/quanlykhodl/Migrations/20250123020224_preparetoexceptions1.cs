using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class preparetoexceptions1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productlocations_products1_id_product",
                table: "productlocations");

            migrationBuilder.DropTable(
                name: "productDeliverynotes");

            migrationBuilder.DropIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations");

            migrationBuilder.AddColumn<int>(
                name: "productsid",
                table: "productlocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "prepareToExports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_product = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CretorEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prepareToExports", x => x.id);
                    table.ForeignKey(
                        name: "FK_prepareToExports_products1_id_product",
                        column: x => x.id_product,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "deliverynotePrepareToEs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_PrepareToExport = table.Column<int>(type: "int", nullable: true),
                    id_delivenote = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CretorEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliverynotePrepareToEs", x => x.id);
                    table.ForeignKey(
                        name: "FK_deliverynotePrepareToEs_deliverynotes_id_delivenote",
                        column: x => x.id_delivenote,
                        principalTable: "deliverynotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_deliverynotePrepareToEs_prepareToExports_id_PrepareToExport",
                        column: x => x.id_PrepareToExport,
                        principalTable: "prepareToExports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_productsid",
                table: "productlocations",
                column: "productsid");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotePrepareToEs_id_delivenote",
                table: "deliverynotePrepareToEs",
                column: "id_delivenote");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotePrepareToEs_id_PrepareToExport",
                table: "deliverynotePrepareToEs",
                column: "id_PrepareToExport");

            migrationBuilder.CreateIndex(
                name: "IX_prepareToExports_id_product",
                table: "prepareToExports",
                column: "id_product");

            migrationBuilder.AddForeignKey(
                name: "FK_productlocations_products1_productsid",
                table: "productlocations",
                column: "productsid",
                principalTable: "products1",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productlocations_products1_productsid",
                table: "productlocations");

            migrationBuilder.DropTable(
                name: "deliverynotePrepareToEs");

            migrationBuilder.DropTable(
                name: "prepareToExports");

            migrationBuilder.DropIndex(
                name: "IX_productlocations_productsid",
                table: "productlocations");

            migrationBuilder.DropColumn(
                name: "productsid",
                table: "productlocations");

            migrationBuilder.CreateTable(
                name: "productDeliverynotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deliverynote = table.Column<int>(type: "int", nullable: true),
                    product_map = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CretorEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productDeliverynotes", x => x.id);
                    table.ForeignKey(
                        name: "FK_productDeliverynotes_deliverynotes_deliverynote",
                        column: x => x.deliverynote,
                        principalTable: "deliverynotes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productDeliverynotes_products1_product_map",
                        column: x => x.product_map,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations",
                column: "id_product");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_deliverynote",
                table: "productDeliverynotes",
                column: "deliverynote");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_product_map",
                table: "productDeliverynotes",
                column: "product_map");

            migrationBuilder.AddForeignKey(
                name: "FK_productlocations_products1_id_product",
                table: "productlocations",
                column: "id_product",
                principalTable: "products1",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
