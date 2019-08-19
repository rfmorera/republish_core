using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class CuentaLastUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Cuenta",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Cuenta");
        }
    }
}
