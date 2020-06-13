using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Analysis
{
    public class AnalysisMessage
    {
        public Fixture Fixture { get; set; }

        public string Sender { get; set; }

        public AnalysisMessage(Fixture fixture)
        {
            Fixture = fixture;
            Sender = "power-lines-fixture-service";
        }
    }
}
