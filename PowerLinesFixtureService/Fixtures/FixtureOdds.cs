using System;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Fixtures
{
    public class FixtureOdds
    {
        public int FixtureId { get; set; }
        public string Country { get; set; }
        public int CountryRank { get; set; }
        public string Division { get; set; }
        public int Tier { get; set; }
        public DateTime Date { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public decimal HomeOdds { get; set; }
        public decimal DrawOdds { get; set; }
        public decimal AwayOdds { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public decimal ExpectedGoals { get; set; }
        public bool IsValid
        {
            get
            {
                return !(HomeOdds == 0 && DrawOdds == 0 && AwayOdds == 0);
            }
        }
        public string Recommended { get; set; }
        public string LowerRecommended { get; set; }
        public DateTime? Calculated { get; set; }

        public FixtureOdds(Fixture fixture)
        {
            var division = new Division(fixture.Division);

            FixtureId = fixture.FixtureId;
            Country = division.Country;
            CountryRank = division.CountryRank;
            Division = division.Name;
            Tier = division.Tier;
            Date = fixture.Date;
            HomeTeam = fixture.HomeTeam;
            AwayTeam = fixture.AwayTeam;
            HomeOdds = fixture.MatchOdds?.Home ?? 0;
            DrawOdds = fixture.MatchOdds?.Draw ?? 0;
            AwayOdds = fixture.MatchOdds?.Away ?? 0;
            HomeGoals = fixture.MatchOdds?.HomeGoals ?? 0;
            AwayGoals = fixture.MatchOdds?.AwayGoals ?? 0;
            ExpectedGoals = fixture.MatchOdds?.ExpectedGoals ?? 0;
            Recommended = fixture.MatchOdds?.Recommended ?? "X";
            LowerRecommended = fixture.MatchOdds?.LowerRecommended ?? "X";
            Calculated = fixture.MatchOdds?.Calculated;
        }
    }
}
