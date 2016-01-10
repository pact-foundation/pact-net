using System;
using PactNet.Logging;

namespace PactNet.Reporters
{
    internal class FileReportOutputter : IReportOutputter
    {
        private readonly Func<ILog> _logFactory;

        public FileReportOutputter(Func<string> loggerNameGenerator)
        {
            _logFactory = () => LogProvider.GetLogger(loggerNameGenerator());
        }

        public void Write(string report)
        {
            _logFactory().Debug(report);
        }
    }
}