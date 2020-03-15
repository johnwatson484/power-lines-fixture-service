using System;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using PowerLinesFixtureService.Analysis;
using Microsoft.EntityFrameworkCore;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        private IConsumer consumer;
        private MessageConfig messageConfig;
        private IServiceScopeFactory serviceScopeFactory;

        public MessageService(IConsumer consumer, MessageConfig messageConfig, IServiceScopeFactory serviceScopeFactory)
        {
            this.consumer = consumer;
            this.messageConfig = messageConfig;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Listen();
            return Task.CompletedTask;
        }

        public void Listen()
        {
            CreateConnectionToQueue();
            consumer.Listen(new Action<string>(ReceiveMessage));
        }

        public void CreateConnectionToQueue()
        {
            Task.Run(() =>
                consumer.CreateConnectionToQueue(new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.FixtureUsername, messageConfig.FixturePassword).ToString(),
                messageConfig.FixtureQueue))
            .Wait();
        }

        private void ReceiveMessage(string message)
        {
            var fixture = JsonConvert.DeserializeObject<Fixture>(message);
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                try
                {
                    dbContext.Fixtures.Add(fixture);
                    dbContext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("{0} v {1} exists, skipping", fixture.HomeTeam, fixture.AwayTeam);
                }
                // var analysisService = scope.ServiceProvider.GetRequiredService<IAnalysisService>();
                // analysisService.GetMatchOdds(fixture.FixtureId);
            }
        }
    }
}
