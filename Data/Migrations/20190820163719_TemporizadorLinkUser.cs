using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class TemporizadorLinkUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Temporizador",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Temporizador_UserId",
                table: "Temporizador",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Temporizador_AspNetUsers_UserId",
                table: "Temporizador",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Temporizador_AspNetUsers_UserId",
                table: "Temporizador");

            migrationBuilder.DropIndex(
                name: "IX_Temporizador_UserId",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Temporizador");
        }
    }
}
