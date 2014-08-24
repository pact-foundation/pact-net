namespace PactNet.Reporters
{
    public interface IReporter
    {
        void ReportInfo(string infoMessage);
        void ReportError(string errorMessage = null, object expected = null, object actual = null);
        void ThrowIfAnyErrors();
        void ClearErrors();
    }
}
