using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PowerLinesFixtureService.Data;

namespace PowerLinesFixtureService.Fixtures
{
    public class FixtureService : IFixtureService
    {
        private readonly ApplicationDbContext dbContext;

        public FixtureService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<FixtureOdds> Get()
        {
            var fixtureOdds = new List<FixtureOdds>();

            var fixtures = dbContext.Fixtures.AsNoTracking().Include(x => x.MatchOdds);

            foreach (var fixture in fixtures)
            {
                fixtureOdds.Add(new FixtureOdds(fixture));
            }

            return fixtureOdds.OrderBy(x => x.Date).ThenBy(x => x.CountryRank).ThenBy(x => x.Tier).ThenBy(x => x.HomeTeam).ToList();
        }
    }
}
