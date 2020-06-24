using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebRubiksCubeTimer.Migrations
{
    public partial class AddPasswordResettable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetRequests",
                columns: table => new
                {
                    RequestId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdentity = table.Column<string>(nullable: true),
                    EmailAdddress = table.Column<string>(nullable: true),
                    ResetKey = table.Column<string>(nullable: true),
                    RequestTime = table.Column<DateTime>(nullable: false),
                    KeyExpires = table.Column<DateTime>(nullable: false),
                    Attempts = table.Column<int>(nullable: false),
                    WasVerified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetRequests", x => x.RequestId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetRequests");
        }
    }
}
