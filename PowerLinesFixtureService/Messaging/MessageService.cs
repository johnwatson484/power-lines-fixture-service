using System;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageService : IMessageService
    {
        private IConnection connection;
        private MessageConfig messageConfig;

        public MessageService(IConnection connection, MessageConfig messageConfig)
        {
            this.connection = connection;
            this.messageConfig = messageConfig;
        }

        public void Listen()
        {
            CreateConnectionToQueue();
            connection.Listen();
        }

        public void CreateConnectionToQueue()
        {
            Task.Run(() =>
                connection.CreateConnectionToQueue(new BrokerUrl(messageConfig.Host, messageConfig.Port, messageConfig.FixtureUsername, messageConfig.FixturePassword).ToString(),
                messageConfig.FixtureQueue))
            .Wait();
        }
    }
}
