using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PowerLinesFixtureService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fixtures",
                columns: table => new
                {
                    fixtureId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    division = table.Column<string>(nullable: true),
                    date = table.Column<DateTime>(nullable: false),
                    homeTeam = table.Column<string>(nullable: true),
                    awayTeam = table.Column<string>(nullable: true),
                    homeOddsAverage = table.Column<decimal>(nullable: false),
                    drawOddsAverage = table.Column<decimal>(nullable: false),
                    awayOddsAverage = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fixtures", x => x.fixtureId);
                });

            migrationBuilder.CreateTable(
                name: "match_odds",
                columns: table => new
                {
                    matchOddsId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fixtureId = table.Column<int>(nullable: false),
                    home = table.Column<decimal>(nullable: false),
                    draw = table.Column<decimal>(nullable: false),
                    away = table.Column<decimal>(nullable: false),
                    expectedHomeGoals = table.Column<int>(nullable: false),
                    expectedAwayGoals = table.Column<int>(nullable: false),
                    expectedGoals = table.Column<decimal>(nullable: false),
                    calculated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_odds", x => x.matchOddsId);
                    table.ForeignKey(
                        name: "FK_match_odds_fixtures_fixtureId",
                        column: x => x.fixtureId,
                        principalTable: "fixtures",
                        principalColumn: "fixtureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fixtures_date_homeTeam_awayTeam",
                table: "fixtures",
                columns: new[] { "date", "homeTeam", "awayTeam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_match_odds_fixtureId",
                table: "match_odds",
                column: "fixtureId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_odds");

            migrationBuilder.DropTable(
                name: "fixtures");
        }
    }
}
