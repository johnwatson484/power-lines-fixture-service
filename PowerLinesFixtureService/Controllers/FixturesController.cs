using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using PowerLinesFixtureService.Fixtures;

namespace PowerLinesFixtureService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FixturesController : ControllerBase
    {
        IFixtureService fixtureService;

        public FixturesController(IFixtureService fixtureService)
        {
            this.fixtureService = fixtureService;
        }
        public ActionResult<IEnumerable<FixtureOdds>> Get()
        {
            return fixtureService.Get();
        }
    }
}
