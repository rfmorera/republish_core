using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class RegistroCosto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Registro",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 23, 23, 35, 8, 514, DateTimeKind.Local));

            migrationBuilder.AddColumn<double>(
                name: "Gasto",
                table: "Registro",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gasto",
                table: "Registro");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Registro",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 23, 23, 35, 8, 514, DateTimeKind.Local),
                oldClrType: typeof(DateTime));
        }
    }
}
