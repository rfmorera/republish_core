using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class UserOptionsRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemporizadoresSystemEnable",
                table: "ClienteOpciones",
                newName: "TemporizadoresUserEnable");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemporizadoresUserEnable",
                table: "ClienteOpciones",
                newName: "TemporizadoresSystemEnable");
        }
    }
}
