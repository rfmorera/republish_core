using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class CaptchaKeysAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "CaptchaKeys",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "CaptchaKeys",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaptchaKeys",
                keyColumn: "Id",
                keyValue: "asd");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "CaptchaKeys");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "CaptchaKeys");
        }
    }
}
