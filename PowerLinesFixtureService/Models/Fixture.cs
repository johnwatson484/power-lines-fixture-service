using System;

namespace PowerLinesFixtureService.Models
{

    public class Fixture
    {
        public int FixtureId { get; set; }
        
        public string Division { get; set; }

        public DateTime Date { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public decimal HomeOddsAverage { get; set; }

        public decimal DrawOddsAverage { get; set; }

        public decimal AwayOddsAverage { get; set; }
    }
}
