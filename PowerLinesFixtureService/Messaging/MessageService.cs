using System;
using Amqp;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        private IConnection connection;
        private MessageConfig messageConfig;
        private IServiceScopeFactory serviceScopeFactory;

        public MessageService(IConnection connection, MessageConfig messageConfig, IServiceScopeFactory serviceScopeFactory)
        {
            this.connection = connection;
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
            var receiver = connection.GetReceiver();
            receiver.Start(
                20,
                (link, message) =>
                {
                    try
                    {
                        Console.WriteLine(message.Body);
                        ReceiveMessage(message);
                        link.Accept(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Message rejected: {0}", ex);
                        link.Reject(message);
                    }
                });
        }

        public void CreateConnectionToQueue()
        {
            Task.Run(() =>
                connection.CreateConnectionToQueue(new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.FixtureUsername, messageConfig.FixturePassword).ToString(),
                messageConfig.FixtureQueue))
            .Wait();
        }

        private void ReceiveMessage(Message message)
        {
            var fixture = JsonConvert.DeserializeObject<Fixture>(message.Body.ToString());
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Fixtures.Add(fixture);
                dbContext.SaveChanges();
            }
        }        
    }
}
