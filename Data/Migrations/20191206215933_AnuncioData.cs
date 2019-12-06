using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class AnuncioData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Enable",
                table: "Anuncio",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "FormUpdateAnuncio",
                table: "Anuncio",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormUpdateAnuncio",
                table: "Anuncio");

            migrationBuilder.AlterColumn<bool>(
                name: "Enable",
                table: "Anuncio",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
