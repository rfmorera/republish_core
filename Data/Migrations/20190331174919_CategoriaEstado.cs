using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class CategoriaEstado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Temporizador",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Categoria",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Categoria");
        }
    }
}
