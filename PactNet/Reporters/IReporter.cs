using PactNet.Comparers;

namespace PactNet.Reporters
{
    internal interface IReporter
    {
        void ReportInfo(string infoMessage);
        void ReportError(string errorMessage = null, object expected = null, object actual = null); //TODO: Can we remove?
        void ReportSummary(ComparisonResult comparisonResult);
        void ReportFailureReasons(ComparisonResult comparisonResult);
        void ThrowIfAnyErrors(); //TODO: Can we remove?
        void ClearErrors(); //TODO: Can we remove?

        void Indent();
        void ResetIndentation();
    }
}
