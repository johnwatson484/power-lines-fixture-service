using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FixtureController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public FixtureController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public ActionResult<IEnumerable<Fixture>> Get()
        {
            return dbContext.Fixtures;
        }
    }
}
