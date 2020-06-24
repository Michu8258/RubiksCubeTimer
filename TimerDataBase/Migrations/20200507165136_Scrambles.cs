using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class Scrambles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scrambles",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryIdentity = table.Column<int>(nullable: false),
                    DefaultScrambleLength = table.Column<int>(nullable: false),
                    MinimumScrambleLength = table.Column<int>(nullable: false),
                    MaximumScrambleLength = table.Column<int>(nullable: false),
                    EliminateDuplicates = table.Column<bool>(nullable: false),
                    AllowRegenerate = table.Column<bool>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    TopColor = table.Column<string>(nullable: false),
                    FrontColor = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scrambles", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_Scrambles_Categories_CategoryIdentity",
                        column: x => x.CategoryIdentity,
                        principalTable: "Categories",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScrambleMoves",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScrambleIdentity = table.Column<long>(nullable: false),
                    Move = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrambleMoves", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_ScrambleMoves_Scrambles_ScrambleIdentity",
                        column: x => x.ScrambleIdentity,
                        principalTable: "Scrambles",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScrambleMoves_ScrambleIdentity",
                table: "ScrambleMoves",
                column: "ScrambleIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Scrambles_CategoryIdentity",
                table: "Scrambles",
                column: "CategoryIdentity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrambleMoves");

            migrationBuilder.DropTable(
                name: "Scrambles");
        }
    }
}
