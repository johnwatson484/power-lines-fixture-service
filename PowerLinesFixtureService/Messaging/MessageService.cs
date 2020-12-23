using System;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PowerLinesMessaging;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        private IConsumer fixtureConsumer;
        private IConsumer oddsConsumer;
        private MessageConfig messageConfig;
        private IServiceScopeFactory serviceScopeFactory;

        public MessageService(IConsumer fixtureConsumer, IConsumer oddsConsumer, MessageConfig messageConfig, IServiceScopeFactory serviceScopeFactory)
        {
            this.fixtureConsumer = fixtureConsumer;
            this.oddsConsumer = oddsConsumer;
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
            fixtureConsumer.Listen(new Action<string>(ReceiveFixtureMessage));
            oddsConsumer.Listen(new Action<string>(ReceiveOddsMessage));
        }

        public void CreateConnectionToQueue()
        {
            fixtureConsumer.CreateConnectionToQueue(QueueType.Worker, new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.FixtureUsername, messageConfig.FixturePassword).ToString(),
                messageConfig.FixtureQueue);
            oddsConsumer.CreateConnectionToQueue(QueueType.ExchangeDirect, new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.OddsUsername, messageConfig.OddsPassword).ToString(),
                messageConfig.OddsQueue, "power-lines-fixture-service");
        }

        private void ReceiveFixtureMessage(string message)
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
            }
        }

        private void ReceiveOddsMessage(string message)
        {
            var matchOdds = JsonConvert.DeserializeObject<MatchOdds>(message);
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.MatchOdds.Upsert(matchOdds)
                    .On(x => new { x.FixtureId })
                    .Run();
            }
        }
    }
}
