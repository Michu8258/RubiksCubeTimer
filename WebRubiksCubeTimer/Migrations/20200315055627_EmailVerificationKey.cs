using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRubiksCubeTimer.Migrations
{
    public partial class EmailVerificationKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationKey",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationKey",
                table: "AspNetUsers");
        }
    }
}
