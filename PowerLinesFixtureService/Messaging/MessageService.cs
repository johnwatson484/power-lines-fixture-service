using System;
using Amqp;
using PowerLinesFixtureService.Data;
using PowerLinesFixtureService.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageService : IMessageService
    {
        private IConnection connection;
        private MessageConfig messageConfig;
        private readonly ApplicationDbContext dbContext;

        public MessageService(IConnection connection, MessageConfig messageConfig, ApplicationDbContext dbContext)
        {
            this.connection = connection;
            this.messageConfig = messageConfig;
            this.dbContext = dbContext;
        }

        public void Listen()
        {
            CreateConnectionToQueue();
            connection.Listen(ReceiveMessage);
        }

        public void CreateConnectionToQueue()
        {
            Task.Run(() =>
                connection.CreateConnectionToQueue(new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.FixtureUsername, messageConfig.FixturePassword).ToString(),
                messageConfig.FixtureQueue))
            .Wait();
        }

        public void ReceiveMessage(Message message)
        {            
            var fixture = JsonConvert.DeserializeObject<Fixture>(message.Body);
            dbContext.Fixtures.Add(fixture);
            dbContext.SaveChanges();
        }
    }
}
