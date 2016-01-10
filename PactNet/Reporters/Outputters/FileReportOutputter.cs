using System;
using PactNet.Logging;

namespace PactNet.Reporters.Outputters
{
    internal class FileReportOutputter : IReportOutputter
    {
        private readonly Func<ILog> _logFactory;

        internal FileReportOutputter(Func<ILog> logFactory)
        {
            _logFactory = logFactory;
        }

        public FileReportOutputter(Func<string> loggerNameGenerator)
            : this(() => LogProvider.GetLogger(loggerNameGenerator()))
        {
        }

        public void Write(string report)
        {
            _logFactory().Debug(report);
        }
    }
}