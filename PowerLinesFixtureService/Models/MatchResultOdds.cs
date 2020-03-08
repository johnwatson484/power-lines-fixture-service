using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerLinesFixtureService.Models
{
    [Table("match_result_odds")]
    public class MatchResultOdds
    {
        [Column("matchResultOddsId")]
        public int MatchResultOddsId { get; set; }

        [Column("fixtureId")]
        public int FixtureId { get; set; }

        [Column("home")]
        public decimal Home { get; set; }

        [Column("draw")]
        public decimal Draw { get; set; }

        [Column("away")]
        public decimal Away { get; set; }
    }
}
