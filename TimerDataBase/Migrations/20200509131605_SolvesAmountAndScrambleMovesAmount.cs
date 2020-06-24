using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class SolvesAmountAndScrambleMovesAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolvesAmount",
                table: "Series",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovesAMount",
                table: "Scrambles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SolvesAmount",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "MovesAMount",
                table: "Scrambles");
        }
    }
}
