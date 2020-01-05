using System;
using System.Threading.Tasks;

namespace PowerLinesFixtureService.Messaging
{
    public interface IConnection
    {
        Task CreateConnectionToQueue(string brokerUrl, string queue);

        void CloseConnection();

        void Listen();
    }
}
