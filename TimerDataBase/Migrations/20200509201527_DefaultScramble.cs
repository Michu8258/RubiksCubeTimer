using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class DefaultScramble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "Scrambles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "Scrambles");
        }
    }
}
