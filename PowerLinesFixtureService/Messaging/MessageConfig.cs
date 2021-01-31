using System;
using System.Configuration;

namespace PowerLinesFixtureService.Messaging
{
    public class MessageConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FixtureQueue { get; set; }
        public string FixtureSubscription { get; set; }
        public string AnalysisQueue { get; set; }
        public string OddsQueue { get; set; }
        public string OddsSubscription { get; set; }
    }
}
