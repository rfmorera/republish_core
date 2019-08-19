using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class RegistroFacturado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Facturado",
                table: "Registro",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facturado",
                table: "Registro");
        }
    }
}
