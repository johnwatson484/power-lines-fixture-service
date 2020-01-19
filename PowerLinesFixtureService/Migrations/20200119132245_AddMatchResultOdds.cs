using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PowerLinesFixtureService.Migrations
{
    public partial class AddMatchResultOdds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fixtures",
                table: "Fixtures");

            migrationBuilder.RenameTable(
                name: "Fixtures",
                newName: "fixtures");

            migrationBuilder.AddPrimaryKey(
                name: "PK_fixtures",
                table: "fixtures",
                column: "FixtureId");

            migrationBuilder.CreateTable(
                name: "match_result_odds",
                columns: table => new
                {
                    MatchResultOddsId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FixtureId = table.Column<int>(nullable: false),
                    Home = table.Column<decimal>(nullable: false),
                    Draw = table.Column<decimal>(nullable: false),
                    Away = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_result_odds", x => x.MatchResultOddsId);
                    table.ForeignKey(
                        name: "FK_match_result_odds_fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "fixtures",
                        principalColumn: "FixtureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_match_result_odds_FixtureId",
                table: "match_result_odds",
                column: "FixtureId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_result_odds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fixtures",
                table: "fixtures");

            migrationBuilder.RenameTable(
                name: "fixtures",
                newName: "Fixtures");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fixtures",
                table: "Fixtures",
                column: "FixtureId");
        }
    }
}
