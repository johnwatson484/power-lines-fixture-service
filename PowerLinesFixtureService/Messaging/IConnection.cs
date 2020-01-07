using System;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;

namespace PowerLinesFixtureService.Messaging
{
    public interface IConnection
    {
        Task CreateConnectionToQueue(string brokerUrl, string queue);

        void CloseConnection();

        void Listen(Action<Message> receiveAction);
    }
}
