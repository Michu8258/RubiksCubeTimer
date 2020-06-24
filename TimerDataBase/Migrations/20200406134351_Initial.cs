using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimerDataBase.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Identity = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Identity);
                });

            migrationBuilder.CreateTable(
                name: "CategoryOptions",
                columns: table => new
                {
                    Identity = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryOptions", x => x.Identity);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Identity = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    FoundationYear = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Identity);
                });

            migrationBuilder.CreateTable(
                name: "PlasticColors",
                columns: table => new
                {
                    Identity = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlasticColors", x => x.Identity);
                });

            migrationBuilder.CreateTable(
                name: "CategoryoptionsSets",
                columns: table => new
                {
                    Identity = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryIdentity = table.Column<int>(nullable: true),
                    OptionIdentity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryoptionsSets", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_CategoryoptionsSets_Categories_CategoryIdentity",
                        column: x => x.CategoryIdentity,
                        principalTable: "Categories",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryoptionsSets_CategoryOptions_OptionIdentity",
                        column: x => x.OptionIdentity,
                        principalTable: "CategoryOptions",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cubes",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryIdentity = table.Column<int>(nullable: true),
                    PlasticColorIdentity = table.Column<int>(nullable: true),
                    ManufacturerIdentity = table.Column<int>(nullable: true),
                    ModelName = table.Column<string>(nullable: false),
                    ReleaseYear = table.Column<int>(nullable: false),
                    Rating = table.Column<double>(nullable: false),
                    RatesAmount = table.Column<long>(nullable: false),
                    WcaPermission = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cubes", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_Cubes_Categories_CategoryIdentity",
                        column: x => x.CategoryIdentity,
                        principalTable: "Categories",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cubes_Manufacturers_ManufacturerIdentity",
                        column: x => x.ManufacturerIdentity,
                        principalTable: "Manufacturers",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cubes_PlasticColors_PlasticColorIdentity",
                        column: x => x.PlasticColorIdentity,
                        principalTable: "PlasticColors",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CubeRatings",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CubeIdentity = table.Column<long>(nullable: true),
                    UserIdentity = table.Column<string>(nullable: false),
                    RateValue = table.Column<int>(nullable: false),
                    Rated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CubeRatings", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_CubeRatings_Cubes_CubeIdentity",
                        column: x => x.CubeIdentity,
                        principalTable: "Cubes",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CubesCollections",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdentity = table.Column<string>(nullable: false),
                    CubeIdentity = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CubesCollections", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_CubesCollections_Cubes_CubeIdentity",
                        column: x => x.CubeIdentity,
                        principalTable: "Cubes",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserIdentity = table.Column<string>(nullable: false),
                    CubeIdentity = table.Column<long>(nullable: true),
                    StartTimeStamp = table.Column<DateTime>(nullable: false),
                    LongestResult = table.Column<TimeSpan>(nullable: false),
                    ShortestResult = table.Column<TimeSpan>(nullable: false),
                    AverageTime = table.Column<TimeSpan>(nullable: false),
                    MeanOf3 = table.Column<TimeSpan>(nullable: false),
                    AverageOf5 = table.Column<TimeSpan>(nullable: false),
                    AtLeastOneDNF = table.Column<bool>(nullable: false),
                    SerieOptionIdentity = table.Column<int>(nullable: true),
                    SerieFinished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_Series_Cubes_CubeIdentity",
                        column: x => x.CubeIdentity,
                        principalTable: "Cubes",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Series_CategoryOptions_SerieOptionIdentity",
                        column: x => x.SerieOptionIdentity,
                        principalTable: "CategoryOptions",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solves",
                columns: table => new
                {
                    Identity = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerieIdentity = table.Column<long>(nullable: true),
                    FinishTimeSpan = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    DNF = table.Column<bool>(nullable: false),
                    PenaltyTwoSeconds = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solves", x => x.Identity);
                    table.ForeignKey(
                        name: "FK_Solves_Series_SerieIdentity",
                        column: x => x.SerieIdentity,
                        principalTable: "Series",
                        principalColumn: "Identity",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryoptionsSets_CategoryIdentity",
                table: "CategoryoptionsSets",
                column: "CategoryIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryoptionsSets_OptionIdentity",
                table: "CategoryoptionsSets",
                column: "OptionIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_CubeRatings_CubeIdentity",
                table: "CubeRatings",
                column: "CubeIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Cubes_CategoryIdentity",
                table: "Cubes",
                column: "CategoryIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Cubes_ManufacturerIdentity",
                table: "Cubes",
                column: "ManufacturerIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Cubes_PlasticColorIdentity",
                table: "Cubes",
                column: "PlasticColorIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_CubesCollections_CubeIdentity",
                table: "CubesCollections",
                column: "CubeIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Series_CubeIdentity",
                table: "Series",
                column: "CubeIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Series_SerieOptionIdentity",
                table: "Series",
                column: "SerieOptionIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_Solves_SerieIdentity",
                table: "Solves",
                column: "SerieIdentity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryoptionsSets");

            migrationBuilder.DropTable(
                name: "CubeRatings");

            migrationBuilder.DropTable(
                name: "CubesCollections");

            migrationBuilder.DropTable(
                name: "Solves");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Cubes");

            migrationBuilder.DropTable(
                name: "CategoryOptions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "PlasticColors");
        }
    }
}
