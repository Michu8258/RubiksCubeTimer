using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class ShortestAcceptableResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ShortestAcceptableResult",
                table: "Categories",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortestAcceptableResult",
                table: "Categories");
        }
    }
}
