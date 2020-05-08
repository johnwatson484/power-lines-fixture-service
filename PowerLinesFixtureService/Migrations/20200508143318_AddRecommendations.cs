using Microsoft.EntityFrameworkCore.Migrations;

namespace PowerLinesFixtureService.Migrations
{
    public partial class AddRecommendations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lowerRecommended",
                table: "match_odds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recommended",
                table: "match_odds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lowerRecommended",
                table: "match_odds");

            migrationBuilder.DropColumn(
                name: "recommended",
                table: "match_odds");
        }
    }
}
