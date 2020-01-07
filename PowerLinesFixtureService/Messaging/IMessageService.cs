using System;

namespace PowerLinesFixtureService.Messaging
{
    public interface IMessageService
    {
        void Listen();
        void CreateConnectionToQueue();
    }
}
