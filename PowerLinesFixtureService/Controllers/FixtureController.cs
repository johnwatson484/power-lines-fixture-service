using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using PowerLinesFixtureService.Services;

namespace PowerLinesFixtureService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FixtureController : ControllerBase
    {
        IFixtureService fixtureService;

        public FixtureController(IFixtureService fixtureService)
        {
            this.fixtureService = fixtureService;
        }
        public ActionResult<IEnumerable<FixtureOdds>> Get()
        {
            return fixtureService.Get();
        }
    }
}
