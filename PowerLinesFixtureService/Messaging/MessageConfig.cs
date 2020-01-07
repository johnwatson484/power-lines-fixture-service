using System;
using System.Configuration;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageConfig
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Transport { get; set; }

        public string FixtureQueue { get; set; }

        public string FixtureUsername { get; set; }

        public string FixturePassword { get; set; }
    }
}
