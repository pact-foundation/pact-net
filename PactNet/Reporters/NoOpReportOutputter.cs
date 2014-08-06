namespace PactNet.Reporters
{
    public class NoOpReportOutputter : IReportOutputter
    {
        public void WriteInfo(string infoMessage, params object[] args)
        {
        }

        public void WriteError(string errorMessage, params object[] args)
        {
        }
    }
}