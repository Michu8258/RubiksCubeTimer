using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerRequestsDataBase.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdentity = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Topic = table.Column<string>(maxLength: 100, nullable: false),
                    Message = table.Column<string>(maxLength: 500, nullable: false),
                    NewChangesByUser = table.Column<bool>(nullable: false),
                    NewChangesByAdmin = table.Column<bool>(nullable: false),
                    CaseClosed = table.Column<bool>(nullable: false),
                    PrivateRequest = table.Column<bool>(nullable: false),
                    RepliesAmount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Identity);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdentity = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    ResponseTime = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Identity);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Responses");
        }
    }
}
