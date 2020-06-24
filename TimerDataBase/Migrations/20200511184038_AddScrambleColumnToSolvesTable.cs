using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class AddScrambleColumnToSolvesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scramble",
                table: "Solves",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scramble",
                table: "Solves");
        }
    }
}
