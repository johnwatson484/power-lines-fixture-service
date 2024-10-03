using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Analysis;

public class AnalysisMessage(Fixture fixture)
{
    public Fixture Fixture { get; set; } = fixture;
    public string Sender { get; set; } = "power-lines-fixture-service";
}

