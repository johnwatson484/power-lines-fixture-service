using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using System.Collections.Generic;
using PowerLinesFixtureService.Messaging;

namespace PowerLinesFixtureService.Analysis
{
    public class AnalysisService : BackgroundService, IAnalysisService
    {
        private IServiceScopeFactory serviceScopeFactory;
        private MessageConfig messageConfig;
        private ISender sender;

        public AnalysisService(IServiceScopeFactory serviceScopeFactory, MessageConfig messageConfig)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.messageConfig = messageConfig;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GetMatchOdds();
            return Task.CompletedTask;
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

            if (pendingFixtures.Count > 0)
            {
                sender = new Sender();
                CreateConnectionToQueue();
                SendFixturesForAnalysis(pendingFixtures);
            }
        }

        public DateTime GetLastResultDate()
        {
            // TODO add get to analysis service
            return DateTime.Now;
        }

        public void CreateConnectionToQueue()
        {
            Task.Run(() =>
                sender.CreateConnectionToQueue(new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.AnalysisUsername, messageConfig.AnalysisPassword).ToString(),
                    messageConfig.AnalysisQueue))
            .Wait();
        }

        public void SendFixturesForAnalysis(List<Fixture> fixtures)
        {
            foreach (var fixture in fixtures)
            {
                sender.SendMessage(fixture);
            }
        }
    }
}
