using System;
using System.Threading.Tasks;

namespace PowerLinesFixtureService.Analysis
{
    public interface IAnalysisApi
    {
        Task<DateTime?> GetLastResultDate();
    }
}
