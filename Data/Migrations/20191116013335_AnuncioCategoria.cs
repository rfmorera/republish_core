using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class AnuncioCategoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Anuncio",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enable",
                table: "Anuncio",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Anuncio");

            migrationBuilder.DropColumn(
                name: "Enable",
                table: "Anuncio");
        }
    }
}
