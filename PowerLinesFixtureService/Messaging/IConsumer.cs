using System;

namespace PowerLinesFixtureService.Messaging
{
    public interface IConsumer
    {
        void CreateConnectionToQueue(string brokerUrl, string queue);

        void CloseConnection();

        void Listen(Action<string> messageAction);
    }
}
