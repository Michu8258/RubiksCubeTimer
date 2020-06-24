using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerRequestsDataBase.Migrations
{
    public partial class MaxMessageLengthSourceChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RequestIdentity",
                table: "Responses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_RequestIdentity",
                table: "Responses",
                column: "RequestIdentity");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Requests_RequestIdentity",
                table: "Responses",
                column: "RequestIdentity",
                principalTable: "Requests",
                principalColumn: "Identity",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Requests_RequestIdentity",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_RequestIdentity",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "RequestIdentity",
                table: "Responses");
        }
    }
}
