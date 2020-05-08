using System.Collections.Generic;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Services
{
    public interface IFixtureService
    {
        List<FixtureOdds> Get();
    }
}
