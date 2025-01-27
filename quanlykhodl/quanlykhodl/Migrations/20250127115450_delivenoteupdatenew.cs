using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class delivenoteupdatenew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deliverynotePrepareToEs_deliverynotes_id_delivenote",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropForeignKey(
                name: "FK_deliverynotePrepareToEs_prepareToExports_id_PrepareToExport",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropForeignKey(
                name: "FK_prepareToExports_accounts_account_id",
                table: "prepareToExports");

            migrationBuilder.DropForeignKey(
                name: "FK_prepareToExports_products1_id_product",
                table: "prepareToExports");

            migrationBuilder.DropIndex(
                name: "IX_deliverynotePrepareToEs_id_delivenote",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prepareToExports",
                table: "prepareToExports");

            migrationBuilder.DropIndex(
                name: "IX_prepareToExports_account_id",
                table: "prepareToExports");

            migrationBuilder.DropIndex(
                name: "IX_prepareToExports_id_product",
                table: "prepareToExports");

            migrationBuilder.RenameTable(
                name: "prepareToExports",
                newName: "PrepareToExport");

            migrationBuilder.AddColumn<bool>(
                name: "isAction",
                table: "deliverynotes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "deliverynotesid",
                table: "deliverynotePrepareToEs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "accountid",
                table: "PrepareToExport",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "productid",
                table: "PrepareToExport",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrepareToExport",
                table: "PrepareToExport",
                column: "id");

            migrationBuilder.CreateTable(
                name: "productDeliverynotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    deliverynote = table.Column<int>(type: "int", nullable: true),
                    product_map = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    area_id = table.Column<int>(type: "int", nullable: true),
                    areaid = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    CretorEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productDeliverynotes", x => x.id);
                    table.ForeignKey(
                        name: "FK_productDeliverynotes_areas_areaid",
                        column: x => x.areaid,
                        principalTable: "areas",
                        principalColumn: "id");
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
                name: "IX_deliverynotePrepareToEs_deliverynotesid",
                table: "deliverynotePrepareToEs",
                column: "deliverynotesid");

            migrationBuilder.CreateIndex(
                name: "IX_PrepareToExport_accountid",
                table: "PrepareToExport",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_PrepareToExport_productid",
                table: "PrepareToExport",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_areaid",
                table: "productDeliverynotes",
                column: "areaid");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_deliverynote",
                table: "productDeliverynotes",
                column: "deliverynote");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_product_map",
                table: "productDeliverynotes",
                column: "product_map");

            migrationBuilder.AddForeignKey(
                name: "FK_deliverynotePrepareToEs_deliverynotes_deliverynotesid",
                table: "deliverynotePrepareToEs",
                column: "deliverynotesid",
                principalTable: "deliverynotes",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_deliverynotePrepareToEs_PrepareToExport_id_PrepareToExport",
                table: "deliverynotePrepareToEs",
                column: "id_PrepareToExport",
                principalTable: "PrepareToExport",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrepareToExport_accounts_accountid",
                table: "PrepareToExport",
                column: "accountid",
                principalTable: "accounts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrepareToExport_products1_productid",
                table: "PrepareToExport",
                column: "productid",
                principalTable: "products1",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deliverynotePrepareToEs_deliverynotes_deliverynotesid",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropForeignKey(
                name: "FK_deliverynotePrepareToEs_PrepareToExport_id_PrepareToExport",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropForeignKey(
                name: "FK_PrepareToExport_accounts_accountid",
                table: "PrepareToExport");

            migrationBuilder.DropForeignKey(
                name: "FK_PrepareToExport_products1_productid",
                table: "PrepareToExport");

            migrationBuilder.DropTable(
                name: "productDeliverynotes");

            migrationBuilder.DropIndex(
                name: "IX_deliverynotePrepareToEs_deliverynotesid",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrepareToExport",
                table: "PrepareToExport");

            migrationBuilder.DropIndex(
                name: "IX_PrepareToExport_accountid",
                table: "PrepareToExport");

            migrationBuilder.DropIndex(
                name: "IX_PrepareToExport_productid",
                table: "PrepareToExport");

            migrationBuilder.DropColumn(
                name: "isAction",
                table: "deliverynotes");

            migrationBuilder.DropColumn(
                name: "deliverynotesid",
                table: "deliverynotePrepareToEs");

            migrationBuilder.DropColumn(
                name: "accountid",
                table: "PrepareToExport");

            migrationBuilder.DropColumn(
                name: "productid",
                table: "PrepareToExport");

            migrationBuilder.RenameTable(
                name: "PrepareToExport",
                newName: "prepareToExports");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prepareToExports",
                table: "prepareToExports",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotePrepareToEs_id_delivenote",
                table: "deliverynotePrepareToEs",
                column: "id_delivenote");

            migrationBuilder.CreateIndex(
                name: "IX_prepareToExports_account_id",
                table: "prepareToExports",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_prepareToExports_id_product",
                table: "prepareToExports",
                column: "id_product");

            migrationBuilder.AddForeignKey(
                name: "FK_deliverynotePrepareToEs_deliverynotes_id_delivenote",
                table: "deliverynotePrepareToEs",
                column: "id_delivenote",
                principalTable: "deliverynotes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_deliverynotePrepareToEs_prepareToExports_id_PrepareToExport",
                table: "deliverynotePrepareToEs",
                column: "id_PrepareToExport",
                principalTable: "prepareToExports",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prepareToExports_accounts_account_id",
                table: "prepareToExports",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prepareToExports_products1_id_product",
                table: "prepareToExports",
                column: "id_product",
                principalTable: "products1",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
