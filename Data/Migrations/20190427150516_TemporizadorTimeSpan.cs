using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class TemporizadorTimeSpan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "NextExecution",
                table: "Temporizador",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HoraInicio",
                table: "Temporizador",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "HoraFin",
                table: "Temporizador",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "NextExecution",
                table: "Temporizador",
                nullable: false,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraInicio",
                table: "Temporizador",
                nullable: false,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<DateTime>(
                name: "HoraFin",
                table: "Temporizador",
                nullable: false,
                oldClrType: typeof(TimeSpan));
        }
    }
}
