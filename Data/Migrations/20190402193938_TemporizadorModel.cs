using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class TemporizadorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Temporizador",
                newName: "Nombre");

            migrationBuilder.AddColumn<bool>(
                name: "Domingo",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Etapa",
                table: "Temporizador",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraFin",
                table: "Temporizador",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "HoraInicio",
                table: "Temporizador",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IntervaloHoras",
                table: "Temporizador",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntervaloMinutos",
                table: "Temporizador",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Jueves",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Lunes",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Martes",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Miercoles",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sabado",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Viernes",
                table: "Temporizador",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domingo",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Etapa",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "HoraFin",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "IntervaloHoras",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "IntervaloMinutos",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Jueves",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Lunes",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Martes",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Miercoles",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Sabado",
                table: "Temporizador");

            migrationBuilder.DropColumn(
                name: "Viernes",
                table: "Temporizador");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Temporizador",
                newName: "Name");
        }
    }
}
