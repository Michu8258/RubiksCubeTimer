using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRubiksCubeTimer.Migrations
{
    public partial class AddColumnForConfirmCodeMatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ResetVerified",
                table: "PasswordResetRequests",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetVerified",
                table: "PasswordResetRequests");
        }
    }
}
