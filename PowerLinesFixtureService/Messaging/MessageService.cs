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
        private ApplicationDbContext dbContext;

        public MessageService(IConnection connection, MessageConfig messageConfig, ApplicationDbContext dbContext)
        {
            this.connection = connection;
            this.messageConfig = messageConfig;
            this.dbContext = dbContext;
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
            dbContext.Fixtures.Add(fixture);
            dbContext.SaveChanges();
        }
    }
}
