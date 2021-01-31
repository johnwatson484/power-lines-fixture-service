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
    public class MessageService : BackgroundService
    {
        private IConnection connection;
        private IConsumer fixtureConsumer;
        private IConsumer oddsConsumer;
        private MessageConfig messageConfig;
        private IServiceScopeFactory serviceScopeFactory;

        public MessageService(MessageConfig messageConfig, IServiceScopeFactory serviceScopeFactory)
        {
            this.messageConfig = messageConfig;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken stoppingToken)
        {
            CreateConnection();
            CreateFixtureConsumer();
            CreateOddsConsumer();

            return base.StartAsync(stoppingToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            fixtureConsumer.Listen(new Action<string>(ReceiveFixtureMessage));
            oddsConsumer.Listen(new Action<string>(ReceiveOddsMessage));
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

        protected void CreateFixtureConsumer()
        {
            var options = new ConsumerOptions
            {
                Name = messageConfig.FixtureQueue,
                QueueName = messageConfig.FixtureQueue,
                SubscriptionQueueName = messageConfig.FixtureSubscription,
                QueueType = QueueType.ExchangeFanout
            };

            fixtureConsumer = connection.CreateConsumerChannel(options);
        }

        protected void CreateOddsConsumer()
        {
            var options = new ConsumerOptions
            {
                Name = messageConfig.OddsQueue,
                QueueName = messageConfig.OddsQueue,
                SubscriptionQueueName = messageConfig.OddsSubscription,
                QueueType = QueueType.ExchangeDirect,
                RoutingKey = "power-lines-fixture-service"
            };

            oddsConsumer = connection.CreateConsumerChannel(options);
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
