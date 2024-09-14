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
using PowerLinesMessaging;

namespace PowerLinesFixtureService.Analysis
{
    public class AnalysisService : BackgroundService
    {
        private IServiceScopeFactory serviceScopeFactory;
        private IAnalysisApi analysisApi;
        private MessageConfig messageConfig;
        private IConnection connection;
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

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            CreateConnection();
            CreateSender();

            return base.StartAsync(stoppingToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(GetMatchOdds, null, TimeSpan.Zero, TimeSpan.FromMinutes(frequencyInMinutes));
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            connection.CloseConnection();
        }

        protected void CreateConnection()
        {
            var options = new ConnectionOptions
            {
                Host = messageConfig.Host,
                Port = messageConfig.Port,
                Username = messageConfig.Username,
                Password = messageConfig.Password
            };
            connection = new Connection(options);
        }

        protected void CreateSender()
        {
            var options = new SenderOptions
            {
                Name = messageConfig.AnalysisQueue,
                QueueName = messageConfig.AnalysisQueue,
                QueueType = QueueType.ExchangeFanout
            };

            sender = connection.CreateSenderChannel(options);
        }

        protected void GetMatchOdds(object state)
        {
            var lastResultDate = GetLastResultDate();

            if (lastResultDate.HasValue)
            {
                CheckPendingFixtures(lastResultDate.Value);
            }
        }

        private DateTime? GetLastResultDate()
        {
            return Task.Run(() => analysisApi.GetLastResultDate()).Result;
        }

        private void CheckPendingFixtures(DateTime lastResultDate)
        {
            List<Fixture> pendingFixtures;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                pendingFixtures = dbContext.Fixtures.AsNoTracking().Include(x => x.MatchOdds).Where(x => x.MatchOdds == null
                || x.MatchOdds.Calculated == null
                || x.MatchOdds.Calculated < lastResultDate
                ).ToList();
            }

            if (pendingFixtures.Count > 0)
            {
                SendFixturesForAnalysis(pendingFixtures);
            }
        }

        private void SendFixturesForAnalysis(List<Fixture> fixtures)
        {
            foreach (var fixture in fixtures)
            {
                sender.SendMessage(new AnalysisMessage(fixture));
            }
        }
    }
}
