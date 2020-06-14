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
using Microsoft.EntityFrameworkCore;

namespace PowerLinesFixtureService.Analysis
{
    public class AnalysisService : BackgroundService, IAnalysisService
    {
        private IServiceScopeFactory serviceScopeFactory;
        private IAnalysisApi analysisApi;
        private MessageConfig messageConfig;
        private ISender sender;
        private Timer timer;
        private int frequencyInMinutes;

        public AnalysisService(IServiceScopeFactory serviceScopeFactory, IAnalysisApi analysisApi, MessageConfig messageConfig, int frequencyInMinutes = 1)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.analysisApi = analysisApi;
            this.messageConfig = messageConfig;
            this.frequencyInMinutes = frequencyInMinutes;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            timer = new Timer(GetMatchOdds, null, TimeSpan.Zero, TimeSpan.FromMinutes(frequencyInMinutes));
            return Task.CompletedTask;
        }

        public void GetMatchOdds(object state)
        {
            var lastResultDate = GetLastResultDate();

            if (lastResultDate.HasValue)
            {
                CheckPendingFixtures(lastResultDate.Value);
            }
        }

        public DateTime? GetLastResultDate()
        {
            return Task.Run(() => analysisApi.GetLastResultDate()).Result;
        }

        public void CheckPendingFixtures(DateTime lastResultDate)
        {
            List<Fixture> pendingFixtures;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                pendingFixtures = dbContext.Fixtures.AsNoTracking().Include(x => x.MatchOdds).Where(x => x.MatchOdds == null || x.MatchOdds.Calculated < lastResultDate).ToList();
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
                sender.SendMessage(new AnalysisMessage(fixture));
            }
        }
    }
}
