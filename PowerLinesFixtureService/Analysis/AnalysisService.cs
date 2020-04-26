using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using System.Collections.Generic;

namespace PowerLinesFixtureService.Analysis
{
    public class AnalysisService : BackgroundService, IAnalysisService
    {
        private IServiceScopeFactory serviceScopeFactory;

        public AnalysisService(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }
        public void GetMatchOdds()
        {
            List<Fixture> pendingFixtures;
            var lastResultDate = GetLastResultDate();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                pendingFixtures = dbContext.Fixtures.Where(x => x.MatchOdds == null || x.MatchOdds.Calculated < lastResultDate).ToList();
            }

            foreach(var fixture in pendingFixtures)
            {

            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GetMatchOdds();
            return Task.CompletedTask;
        }

        public DateTime GetLastResultDate()
        {
            // TODO add get to analysis service
            return DateTime.Now;
        }
    }
}
