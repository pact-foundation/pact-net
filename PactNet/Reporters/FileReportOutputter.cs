using PactNet.Logging;

namespace PactNet.Reporters
{
    internal class FileReportOutputter
    {
        private readonly ILog _log;

        public FileReportOutputter(string loggerName)
        {
            _log = LogProvider.GetLogger(loggerName);
        }

        public void Write(string message)
        {
            _log.Debug(message);
        }
    }
}