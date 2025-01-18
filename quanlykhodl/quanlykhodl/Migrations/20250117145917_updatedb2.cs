using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatedb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_accounts_accountid",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageProduct_products1_products_idid",
                table: "ImageProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_warehouses_accounts_accountid",
                table: "warehouses");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_warehouses_accountid",
                table: "warehouses");

            migrationBuilder.DropIndex(
                name: "IX_categories_accountid",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageProduct",
                table: "ImageProduct");

            migrationBuilder.DropColumn(
                name: "accountid",
                table: "warehouses");

            migrationBuilder.DropColumn(
                name: "accountid",
                table: "categories");

            migrationBuilder.RenameTable(
                name: "ImageProduct",
                newName: "imageProducts");

            migrationBuilder.RenameIndex(
                name: "IX_ImageProduct_products_idid",
                table: "imageProducts",
                newName: "IX_imageProducts_products_idid");

            migrationBuilder.AddColumn<int>(
                name: "account_id",
                table: "suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isConfirmation",
                table: "plans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isConsent",
                table: "plans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "account_id",
                table: "floors",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_imageProducts",
                table: "imageProducts",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_account_map",
                table: "warehouses",
                column: "account_map");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_account_id",
                table: "suppliers",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_floors_account_id",
                table: "floors",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_account_Id",
                table: "categories",
                column: "account_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_accounts_account_Id",
                table: "categories",
                column: "account_Id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_floors_accounts_account_id",
                table: "floors",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_imageProducts_products1_products_idid",
                table: "imageProducts",
                column: "products_idid",
                principalTable: "products1",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_accounts_account_id",
                table: "suppliers",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_warehouses_accounts_account_map",
                table: "warehouses",
                column: "account_map",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_accounts_account_Id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_floors_accounts_account_id",
                table: "floors");

            migrationBuilder.DropForeignKey(
                name: "FK_imageProducts_products1_products_idid",
                table: "imageProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_accounts_account_id",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_warehouses_accounts_account_map",
                table: "warehouses");

            migrationBuilder.DropIndex(
                name: "IX_warehouses_account_map",
                table: "warehouses");

            migrationBuilder.DropIndex(
                name: "IX_suppliers_account_id",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "IX_floors_account_id",
                table: "floors");

            migrationBuilder.DropIndex(
                name: "IX_categories_account_Id",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_imageProducts",
                table: "imageProducts");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "isConfirmation",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "isConsent",
                table: "plans");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "floors");

            migrationBuilder.RenameTable(
                name: "imageProducts",
                newName: "ImageProduct");

            migrationBuilder.RenameIndex(
                name: "IX_imageProducts_products_idid",
                table: "ImageProduct",
                newName: "IX_ImageProduct_products_idid");

            migrationBuilder.AddColumn<int>(
                name: "accountid",
                table: "warehouses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "accountid",
                table: "categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageProduct",
                table: "ImageProduct",
                column: "id");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plan_map = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CretorEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    isConfirmation = table.Column<bool>(type: "bit", nullable: false),
                    isConsent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_notifications_plans_plan_map",
                        column: x => x.plan_map,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_accountid",
                table: "warehouses",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_categories_accountid",
                table: "categories",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_plan_map",
                table: "notifications",
                column: "plan_map");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_accounts_accountid",
                table: "categories",
                column: "accountid",
                principalTable: "accounts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageProduct_products1_products_idid",
                table: "ImageProduct",
                column: "products_idid",
                principalTable: "products1",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_warehouses_accounts_accountid",
                table: "warehouses",
                column: "accountid",
                principalTable: "accounts",
                principalColumn: "id");
        }
    }
}
