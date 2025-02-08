using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlykhodl.Migrations
{
    public partial class updatedb28 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "retailcustomers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Retailcustomers", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usertokenapps",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userTokenApps", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_accounts_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    public_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "deliverynotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    retailcustomers = table.Column<int>(type: "int", nullable: true),
                    accountmap = table.Column<int>(type: "int", nullable: true),
                    isRetailcustomers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    price = table.Column<double>(type: "double", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<double>(type: "double", nullable: false),
                    deliveryaddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tax = table.Column<int>(type: "int", nullable: true),
                    ispercentage = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    isaction = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ispack = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliverynotes", x => x.id);
                    table.ForeignKey(
                        name: "FK_deliverynotes_accounts_accountmap",
                        column: x => x.accountmap,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_deliverynotes_Retailcustomers_retailcustomers",
                        column: x => x.retailcustomers,
                        principalTable: "retailcustomers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "importforms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tite = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_idmap = table.Column<int>(type: "int", nullable: true),
                    isproductnew = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isaction = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    tax = table.Column<int>(type: "int", nullable: true),
                    actualquantity = table.Column<int>(type: "int", nullable: true),
                    ispercentage = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    price = table.Column<double>(type: "double", nullable: false),
                    total = table.Column<double>(type: "double", nullable: false),
                    deliveryaddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_importforms", x => x.id);
                    table.ForeignKey(
                        name: "FK_importforms_accounts_account_idmap",
                        column: x => x.account_idmap,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    senderid = table.Column<int>(type: "int", nullable: true),
                    receiverid = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isread = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_Messages_accounts_receiverid",
                        column: x => x.receiverid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_accounts_senderid",
                        column: x => x.senderid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "onlineusersuser",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    connectionid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isonline = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_onlineUsersUser", x => x.id);
                    table.ForeignKey(
                        name: "FK_onlineUsersUser_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.id);
                    table.ForeignKey(
                        name: "FK_suppliers_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "warehouses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numberoffloors = table.Column<int>(type: "int", nullable: true),
                    street = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    district = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_map = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.id);
                    table.ForeignKey(
                        name: "FK_warehouses_accounts_account_map",
                        column: x => x.account_map,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user1id = table.Column<int>(type: "int", nullable: false),
                    user2id = table.Column<int>(type: "int", nullable: false),
                    lastmessageid = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Conversations_accounts_user1id",
                        column: x => x.user1id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_accounts_user2id",
                        column: x => x.user2id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Messages_lastmessageid",
                        column: x => x.lastmessageid,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "products1",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<double>(type: "double", nullable: false),
                    donvitinh = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    star = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_map = table.Column<int>(type: "int", nullable: true),
                    account_map = table.Column<int>(type: "int", nullable: true),
                    suppliers = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products1", x => x.id);
                    table.ForeignKey(
                        name: "FK_products1_accounts_account_map",
                        column: x => x.account_map,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_products1_categories_category_map",
                        column: x => x.category_map,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_products1_suppliers_suppliers",
                        column: x => x.suppliers,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "floors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantityarea = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    warehouse = table.Column<int>(type: "int", nullable: true),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_floors", x => x.id);
                    table.ForeignKey(
                        name: "FK_floors_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_floors_warehouses_warehouse",
                        column: x => x.warehouse,
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "userconversations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    userconversationid = table.Column<int>(type: "int", nullable: false),
                    userid = table.Column<int>(type: "int", nullable: false),
                    conversationid = table.Column<int>(type: "int", nullable: false),
                    isdeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userConversations", x => x.id);
                    table.ForeignKey(
                        name: "FK_userConversations_accounts_userid",
                        column: x => x.userid,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userConversations_Conversations_conversationid",
                        column: x => x.conversationid,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "imageproducts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    productmap = table.Column<int>(type: "int", nullable: true),
                    public_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    link = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imageProducts", x => x.id);
                    table.ForeignKey(
                        name: "FK_imageProducts_products1_productmap",
                        column: x => x.productmap,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PrepareToExport",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_product = table.Column<int>(type: "int", nullable: true),
                    productid = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    ischeck = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    accountid = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepareToExport", x => x.id);
                    table.ForeignKey(
                        name: "FK_PrepareToExport_accounts_accountid",
                        column: x => x.accountid,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_PrepareToExport_products1_productid",
                        column: x => x.productid,
                        principalTable: "products1",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    storage = table.Column<int>(type: "int", nullable: false),
                    floor = table.Column<int>(type: "int", nullable: true),
                    account_id = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.id);
                    table.ForeignKey(
                        name: "FK_areas_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_areas_floors_floor",
                        column: x => x.floor,
                        principalTable: "floors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "deliverynotepreparetoes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_preparetoexport = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_delivenote = table.Column<int>(type: "int", nullable: true),
                    deliverynotesid = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliverynotePrepareToEs", x => x.id);
                    table.ForeignKey(
                        name: "FK_deliverynotePrepareToEs_deliverynotes_deliverynotesid",
                        column: x => x.deliverynotesid,
                        principalTable: "deliverynotes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_deliverynotePrepareToEs_PrepareToExport_id_preparetoexport",
                        column: x => x.id_preparetoexport,
                        principalTable: "PrepareToExport",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "shelfs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    max = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    account = table.Column<int>(type: "int", nullable: true),
                    area = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shelfs", x => x.id);
                    table.ForeignKey(
                        name: "FK_shelfs_accounts_account",
                        column: x => x.account,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shelfs_areas_area",
                        column: x => x.area,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "codelocations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_helf = table.Column<int>(type: "int", nullable: true),
                    location = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codelocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_codelocations_shelfs_id_helf",
                        column: x => x.id_helf,
                        principalTable: "shelfs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "locationexceptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_shelf = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: true),
                    max = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locationExceptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_locationExceptions_shelfs_id_shelf",
                        column: x => x.id_shelf,
                        principalTable: "shelfs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "productimportforms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    importform = table.Column<int>(type: "int", nullable: true),
                    product = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    supplier = table.Column<int>(type: "int", nullable: true),
                    location = table.Column<int>(type: "int", nullable: true),
                    shelf_id = table.Column<int>(type: "int", nullable: true),
                    isaction = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productImportforms", x => x.id);
                    table.ForeignKey(
                        name: "FK_productImportforms_importforms_importform",
                        column: x => x.importform,
                        principalTable: "importforms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productImportforms_products1_product",
                        column: x => x.product,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productImportforms_shelfs_shelf_id",
                        column: x => x.shelf_id,
                        principalTable: "shelfs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productImportforms_suppliers_supplier",
                        column: x => x.supplier,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "productlocations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    location = table.Column<int>(type: "int", nullable: false),
                    id_product = table.Column<int>(type: "int", nullable: false),
                    id_shelf = table.Column<int>(type: "int", nullable: false),
                    isaction = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    codelocation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productlocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_productlocations_products1_id_product",
                        column: x => x.id_product,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productlocations_shelfs_id_shelf",
                        column: x => x.id_shelf,
                        principalTable: "shelfs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    localtionnew = table.Column<int>(type: "int", nullable: true),
                    isconfirmation = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    isconsent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    iswarehourse = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    productlocation_map = table.Column<int>(type: "int", nullable: true),
                    Receiver = table.Column<int>(type: "int", nullable: true),
                    localtionold = table.Column<int>(type: "int", nullable: true),
                    warehouseold = table.Column<int>(type: "int", nullable: true),
                    shelfOld = table.Column<int>(type: "int", nullable: true),
                    areaold = table.Column<int>(type: "int", nullable: true),
                    floorold = table.Column<int>(type: "int", nullable: true),
                    warehouse = table.Column<int>(type: "int", nullable: true),
                    shelf = table.Column<int>(type: "int", nullable: true),
                    floor = table.Column<int>(type: "int", nullable: true),
                    area = table.Column<int>(type: "int", nullable: true),
                    warehouse_idid = table.Column<int>(type: "int", nullable: true),
                    shelfidid = table.Column<int>(type: "int", nullable: true),
                    floor_idid = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.id);
                    table.ForeignKey(
                        name: "FK_plans_accounts_Receiver",
                        column: x => x.Receiver,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plans_areas_area",
                        column: x => x.area,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plans_floors_floor_idid",
                        column: x => x.floor_idid,
                        principalTable: "floors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plans_productlocations_productlocation_map",
                        column: x => x.productlocation_map,
                        principalTable: "productlocations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plans_shelfs_shelfidid",
                        column: x => x.shelfidid,
                        principalTable: "shelfs",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plans_warehouses_warehouse_idid",
                        column: x => x.warehouse_idid,
                        principalTable: "warehouses",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "productdeliverynotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    deliverynote = table.Column<int>(type: "int", nullable: true),
                    product_map = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    shelf_id = table.Column<int>(type: "int", nullable: true),
                    productlocation_id = table.Column<int>(type: "int", nullable: true),
                    productlocationsid = table.Column<int>(type: "int", nullable: true),
                    shelfsid = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
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
                        name: "FK_productDeliverynotes_productlocations_productlocationsid",
                        column: x => x.productlocationsid,
                        principalTable: "productlocations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_productDeliverynotes_products1_product_map",
                        column: x => x.product_map,
                        principalTable: "products1",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_productDeliverynotes_shelfs_shelfsid",
                        column: x => x.shelfsid,
                        principalTable: "shelfs",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "warehousetransferstatuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    plan = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehousetransferstatuses", x => x.id);
                    table.ForeignKey(
                        name: "FK_warehousetransferstatuses_plans_plan",
                        column: x => x.plan,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "statusitems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    icon = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    warehousetransferstatus = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statusItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_statusItems_warehousetransferstatuses_warehousetransferstatus",
                        column: x => x.warehousetransferstatus,
                        principalTable: "warehousetransferstatuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "imagestatusitems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    statusitemmap = table.Column<int>(type: "int", nullable: true),
                    image = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    publicid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    cretoredit = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createdat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updatedat = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagestatusitems", x => x.id);
                    table.ForeignKey(
                        name: "FK_imagestatusitems_statusItems_statusitemmap",
                        column: x => x.statusitemmap,
                        principalTable: "statusitems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_accounts_role_id",
                table: "accounts",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_areas_account_id",
                table: "areas",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_areas_floor",
                table: "areas",
                column: "floor");

            migrationBuilder.CreateIndex(
                name: "IX_categories_account_id",
                table: "categories",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_codelocations_id_helf",
                table: "codelocations",
                column: "id_helf");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_lastmessageid",
                table: "conversations",
                column: "lastmessageid");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_user1id",
                table: "conversations",
                column: "user1id");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_user2id",
                table: "conversations",
                column: "user2id");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotePrepareToEs_deliverynotesid",
                table: "deliverynotepreparetoes",
                column: "deliverynotesid");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotePrepareToEs_id_preparetoexport",
                table: "deliverynotepreparetoes",
                column: "id_preparetoexport");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotes_accountmap",
                table: "deliverynotes",
                column: "accountmap");

            migrationBuilder.CreateIndex(
                name: "IX_deliverynotes_retailcustomers",
                table: "deliverynotes",
                column: "retailcustomers");

            migrationBuilder.CreateIndex(
                name: "IX_floors_account_id",
                table: "floors",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_floors_warehouse",
                table: "floors",
                column: "warehouse");

            migrationBuilder.CreateIndex(
                name: "IX_imageProducts_productmap",
                table: "imageproducts",
                column: "productmap");

            migrationBuilder.CreateIndex(
                name: "IX_imagestatusitems_statusitemmap",
                table: "imagestatusitems",
                column: "statusitemmap");

            migrationBuilder.CreateIndex(
                name: "IX_importforms_account_idmap",
                table: "importforms",
                column: "account_idmap");

            migrationBuilder.CreateIndex(
                name: "IX_locationExceptions_id_shelf",
                table: "locationexceptions",
                column: "id_shelf");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_receiverid",
                table: "messages",
                column: "receiverid");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_senderid",
                table: "messages",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_onlineUsersUser_account_id",
                table: "onlineusersuser",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_plans_area",
                table: "plans",
                column: "area");

            migrationBuilder.CreateIndex(
                name: "IX_plans_floor_idid",
                table: "plans",
                column: "floor_idid");

            migrationBuilder.CreateIndex(
                name: "IX_plans_productlocation_map",
                table: "plans",
                column: "productlocation_map");

            migrationBuilder.CreateIndex(
                name: "IX_plans_Receiver",
                table: "plans",
                column: "Receiver");

            migrationBuilder.CreateIndex(
                name: "IX_plans_shelfidid",
                table: "plans",
                column: "shelfidid");

            migrationBuilder.CreateIndex(
                name: "IX_plans_warehouse_idid",
                table: "plans",
                column: "warehouse_idid");

            migrationBuilder.CreateIndex(
                name: "IX_PrepareToExport_accountid",
                table: "PrepareToExport",
                column: "accountid");

            migrationBuilder.CreateIndex(
                name: "IX_PrepareToExport_productid",
                table: "PrepareToExport",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_deliverynote",
                table: "productdeliverynotes",
                column: "deliverynote");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_product_map",
                table: "productdeliverynotes",
                column: "product_map");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_productlocationsid",
                table: "productdeliverynotes",
                column: "productlocationsid");

            migrationBuilder.CreateIndex(
                name: "IX_productDeliverynotes_shelfsid",
                table: "productdeliverynotes",
                column: "shelfsid");

            migrationBuilder.CreateIndex(
                name: "IX_productImportforms_importform",
                table: "productimportforms",
                column: "importform");

            migrationBuilder.CreateIndex(
                name: "IX_productImportforms_product",
                table: "productimportforms",
                column: "product");

            migrationBuilder.CreateIndex(
                name: "IX_productImportforms_shelf_id",
                table: "productimportforms",
                column: "shelf_id");

            migrationBuilder.CreateIndex(
                name: "IX_productImportforms_supplier",
                table: "productimportforms",
                column: "supplier");

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_id_product",
                table: "productlocations",
                column: "id_product");

            migrationBuilder.CreateIndex(
                name: "IX_productlocations_id_shelf",
                table: "productlocations",
                column: "id_shelf");

            migrationBuilder.CreateIndex(
                name: "IX_products1_account_map",
                table: "products1",
                column: "account_map");

            migrationBuilder.CreateIndex(
                name: "IX_products1_category_map",
                table: "products1",
                column: "category_map");

            migrationBuilder.CreateIndex(
                name: "IX_products1_suppliers",
                table: "products1",
                column: "suppliers");

            migrationBuilder.CreateIndex(
                name: "IX_shelfs_account",
                table: "shelfs",
                column: "account");

            migrationBuilder.CreateIndex(
                name: "IX_shelfs_area",
                table: "shelfs",
                column: "area");

            migrationBuilder.CreateIndex(
                name: "IX_statusItems_warehousetransferstatus",
                table: "statusitems",
                column: "warehousetransferstatus");

            migrationBuilder.CreateIndex(
                name: "IX_suppliers_account_id",
                table: "suppliers",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_account_id",
                table: "tokens",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_userConversations_conversationid",
                table: "userconversations",
                column: "conversationid");

            migrationBuilder.CreateIndex(
                name: "IX_userConversations_userid",
                table: "userconversations",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_account_map",
                table: "warehouses",
                column: "account_map");

            migrationBuilder.CreateIndex(
                name: "IX_warehousetransferstatuses_plan",
                table: "warehousetransferstatuses",
                column: "plan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "codelocations");

            migrationBuilder.DropTable(
                name: "deliverynotepreparetoes");

            migrationBuilder.DropTable(
                name: "imageproducts");

            migrationBuilder.DropTable(
                name: "imagestatusitems");

            migrationBuilder.DropTable(
                name: "locationexceptions");

            migrationBuilder.DropTable(
                name: "onlineusersuser");

            migrationBuilder.DropTable(
                name: "productdeliverynotes");

            migrationBuilder.DropTable(
                name: "productimportforms");

            migrationBuilder.DropTable(
                name: "tokens");

            migrationBuilder.DropTable(
                name: "userconversations");

            migrationBuilder.DropTable(
                name: "usertokenapps");

            migrationBuilder.DropTable(
                name: "PrepareToExport");

            migrationBuilder.DropTable(
                name: "statusitems");

            migrationBuilder.DropTable(
                name: "deliverynotes");

            migrationBuilder.DropTable(
                name: "importforms");

            migrationBuilder.DropTable(
                name: "conversations");

            migrationBuilder.DropTable(
                name: "warehousetransferstatuses");

            migrationBuilder.DropTable(
                name: "retailcustomers");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "productlocations");

            migrationBuilder.DropTable(
                name: "products1");

            migrationBuilder.DropTable(
                name: "shelfs");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.DropTable(
                name: "floors");

            migrationBuilder.DropTable(
                name: "warehouses");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
