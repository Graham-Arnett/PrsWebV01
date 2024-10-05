using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrsEfWebApi.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "varchar(75)", unicode: false, maxLength: 75, nullable: false),
                    REVIEWER = table.Column<bool>(type: "bit", nullable: false),
                    ADMIN = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__3214EC07EE222D60", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    State = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    Zip = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vendor__3214EC071BB06010", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Justification = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    DateNeeded = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryMode = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "New"),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ReasonForRejection = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Request__3214EC07076C2F41", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Request__UserId__33D4B598",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Unit = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    PhotoPath = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product__3214EC075EF92D45", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Product__VendorI__2F10007B",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LineItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LineItem__3214EC07B81D1951", x => x.Id);
                    table.ForeignKey(
                        name: "FK__LineItem__Produc__37A5467C",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__LineItem__Reques__38996AB5",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineItem_ProductId",
                table: "LineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "UQ_LineItem_Req_Prod",
                table: "LineItem",
                columns: new[] { "RequestId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Vendor_Part",
                table: "Product",
                columns: new[] { "VendorId", "PartNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_UserId",
                table: "Request",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_User_Person",
                table: "User",
                columns: new[] { "FirstName", "LastName", "PhoneNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Vendor_Code",
                table: "Vendor",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineItem");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
