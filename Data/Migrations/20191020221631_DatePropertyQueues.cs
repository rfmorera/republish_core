using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlueDot.Data.Migrations
{
    public partial class DatePropertyQueues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ShortQueue",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "LongQueue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "ShortQueue");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "LongQueue");
        }
    }
}
