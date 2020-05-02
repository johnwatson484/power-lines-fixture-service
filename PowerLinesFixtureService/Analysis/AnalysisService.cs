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
        private IAnalysisApi analysisApi;
        private MessageConfig messageConfig;
        private ISender sender;

        public AnalysisService(IServiceScopeFactory serviceScopeFactory, IAnalysisApi analysisApi, MessageConfig messageConfig)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.analysisApi = analysisApi;
            this.messageConfig = messageConfig;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GetMatchOdds();
            return Task.CompletedTask;
        }

        public void GetMatchOdds()
        {
            var lastResultDate = GetLastResultDate();

            if (lastResultDate.HasValue)
            {
                CheckPendingResults(lastResultDate.Value);
            }
        }

        public DateTime? GetLastResultDate()
        {
            return Task.Run(() => analysisApi.GetLastResultDate()).Result;
        }

        public void CheckPendingResults(DateTime lastResultDate)
        {
            List<Fixture> pendingFixtures;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                pendingFixtures = dbContext.Fixtures.Where(x => x.MatchOdds == null || x.MatchOdds.Calculated < lastResultDate).ToList();
            }

            if (pendingFixtures.Count > 0)
            {
                SendFixturesForAnalysis(pendingFixtures);
            }
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
            sender = new Sender();
            CreateConnectionToQueue();

            foreach (var fixture in fixtures)
            {
                sender.SendMessage(fixture);
            }
        }
    }
}
