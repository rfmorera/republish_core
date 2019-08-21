using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class UserOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClienteOpciones",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TemporizadoresSystemEnable = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteOpciones", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteOpciones");
        }
    }
}
