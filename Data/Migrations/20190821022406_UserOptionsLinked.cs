using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class UserOptionsLinked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ClienteOpciones",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteOpciones_UserId",
                table: "ClienteOpciones",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClienteOpciones_AspNetUsers_UserId",
                table: "ClienteOpciones",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClienteOpciones_AspNetUsers_UserId",
                table: "ClienteOpciones");

            migrationBuilder.DropIndex(
                name: "IX_ClienteOpciones_UserId",
                table: "ClienteOpciones");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClienteOpciones");
        }
    }
}
