using System.Collections.Generic;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Services
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
            throw new System.NotImplementedException();
        }
    }
}
