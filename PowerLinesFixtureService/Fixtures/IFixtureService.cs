using System.Collections.Generic;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Fixtures
{
    public interface IFixtureService
    {
        List<FixtureOdds> Get();
    }
}
