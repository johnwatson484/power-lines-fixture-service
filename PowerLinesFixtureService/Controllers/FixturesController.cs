using Microsoft.AspNetCore.Mvc;
using PowerLinesFixtureService.Fixtures;

namespace PowerLinesFixtureService.Controllers;

[ApiController]
[Route("[controller]")]
public class FixturesController(IFixtureService fixtureService) : ControllerBase
{
    readonly IFixtureService fixtureService = fixtureService;

    [Route("")]
    [HttpGet]
    public ActionResult<IEnumerable<FixtureOdds>> Index()
    {
        return fixtureService.Get();
    }
}
