using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Newtonsoft.Json;

namespace PowerLinesFixtureService.Messaging
{
    public class AmqpConnection : IConnection
    {
        protected ConnectionFactory factory;
        protected Address brokerAddr;
        protected Connection connection;
        protected Session session;
        protected ReceiverLink receiver;

        public async Task CreateConnectionToQueue(string brokerUrl, string queue)
        {
            if (factory == null)
            {
                ConfigureConnectionFactory();
            }
            Console.WriteLine(brokerUrl);
            brokerAddr = new Address(brokerUrl);
            connection = await factory.CreateAsync(brokerAddr);
            session = new Session(connection);
            receiver = new ReceiverLink(session, "receiver", queue);
        }

        private void ConfigureConnectionFactory()
        {
            factory = new ConnectionFactory();
            factory.AMQP.ContainerId = "fixture-service-container";
        }

        public void CloseConnection()
        {
            receiver.Close();
            session.Close();
            connection.Close();
        }

        public void Listen(Action<Message> receiveAction)
        {
            receiver.Start(
                20,
                (link, message) =>
                {
                    try
                    {
                        Console.WriteLine(message.Body);
                        receiveAction(message);
                        link.Accept(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Message rejected: {0}", ex);
                        link.Reject(message);
                    }
                });
        }
    }
}
