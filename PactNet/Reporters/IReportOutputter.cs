namespace PactNet.Reporters
{
    public interface IReportOutputter
    {
        void WriteInfo(string infoMessage, params object[] args);
        void WriteError(string errorMessage, params object[] args);
    }
}