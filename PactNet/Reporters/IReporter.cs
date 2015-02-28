using PactNet.Comparers;

namespace PactNet.Reporters
{
    public interface IReporter
    {
        void ReportInfo(string infoMessage, int depth = 0);
        void ReportError(string errorMessage = null, object expected = null, object actual = null);
        void ReportComparisonResult(ComparisonResult comparisonResult);
        void GenerateSummary(ComparisonResult comparisonResult);
        void ReportFailureReasons(ComparisonResult comparisonResult);
        void ThrowIfAnyErrors();
        void ClearErrors();
    }
}
