using Microsoft.EntityFrameworkCore;
using PowerLinesFixtureService.Data;

namespace PowerLinesFixtureService.Fixtures;

public class FixtureService(ApplicationDbContext dbContext) : IFixtureService
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public List<FixtureOdds> Get()
    {
        var fixtureOdds = new List<FixtureOdds>();
        var startDate = DateTime.UtcNow.AddDays(-1).Date;

        var fixtures = dbContext.Fixtures.AsNoTracking().Include(x => x.MatchOdds).Where(x => x.Date >= startDate && x.MatchOdds.Calculated != null);

        foreach (var fixture in fixtures)
        {
            fixtureOdds.Add(new FixtureOdds(fixture));
        }

        return fixtureOdds.OrderBy(x => x.Date).ThenBy(x => x.CountryRank).ThenBy(x => x.Tier).ThenBy(x => x.HomeTeam).ToList();
    }
}
