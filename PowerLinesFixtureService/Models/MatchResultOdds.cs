using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerLinesFixtureService.Models
{
    [Table("match_result_odds")]
    public class MatchResultOdds
    {
        public int MatchResultOddsId { get; set; }

        public int FixtureId { get; set; }

        public decimal Home { get; set; }

        public decimal Draw { get; set; }

        public decimal Away { get; set; }
    }
}
