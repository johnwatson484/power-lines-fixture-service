namespace PowerLinesFixtureService.Analysis;

public interface IAnalysisApi
{
    Task<DateTime?> GetLastResultDate();
}
