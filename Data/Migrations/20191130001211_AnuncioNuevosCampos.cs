using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class AnuncioNuevosCampos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Anuncio",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "Procesando",
                table: "Anuncio",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Revalidado",
                table: "Anuncio",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Anuncio");

            migrationBuilder.DropColumn(
                name: "Procesando",
                table: "Anuncio");

            migrationBuilder.DropColumn(
                name: "Revalidado",
                table: "Anuncio");
        }
    }
}
