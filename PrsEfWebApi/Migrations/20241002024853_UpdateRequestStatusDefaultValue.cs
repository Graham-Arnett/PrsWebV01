using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrsEfWebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestStatusDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Request",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true,
                defaultValue: "NEW",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldDefaultValue: "New");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Request",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "New",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "NEW");
        }
    }
}
