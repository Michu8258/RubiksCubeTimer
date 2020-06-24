using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class ScrambleName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovesAMount",
                table: "Scrambles",
                newName: "MovesAmount");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Scrambles",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Scrambles");

            migrationBuilder.RenameColumn(
                name: "MovesAmount",
                table: "Scrambles",
                newName: "MovesAMount");
        }
    }
}
