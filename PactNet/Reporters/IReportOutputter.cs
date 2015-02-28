namespace PactNet.Reporters
{
    public interface IReportOutputter
    {
        void WriteInfo(string infoMessage, int tabDepth = 0);
        void WriteError(string errorMessage, int tabDepth = 0);
        void WriteSuccess(string successMessage, int tabDepth = 0);
    }
}