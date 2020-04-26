using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerLinesFixtureService.Models
{
    [Table("match_odds")]
    public class MatchOdds
    {
        [Column("matchOddsId")]
        public int MatchOddsId { get; set; }

        [Column("fixtureId")]
        public int FixtureId { get; set; }

        [Column("home")]
        public decimal Home { get; set; }

        [Column("draw")]
        public decimal Draw { get; set; }

        [Column("away")]
        public decimal Away { get; set; }

        [Column("expectedHomeGoals")]
        public int HomeGoals { get; set; }

        [Column("expectedAwayGoals")]
        public int AwayGoals { get; set; }

        [Column("expectedGoals")]
        public decimal ExpectedGoals { get; set; }

        [Column("calculated")]
        public DateTime Calculated { get; set; }
    }
}
