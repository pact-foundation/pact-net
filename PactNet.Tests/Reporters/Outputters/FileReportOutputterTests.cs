using System.Collections.Generic;
using NSubstitute;
using PactNet.Logging;
using PactNet.Reporters;
using PactNet.Reporters.Outputters;
using Xunit;

namespace PactNet.Tests.Reporters.Outputters
{
    public class FileReportOutputterTests
    {
        private ILog _log;

        private IReportOutputter GetSubject()
        {
            _log = Substitute.For<ILog>();

            return new FileReportOutputter(() => _log);
        }

        [Fact]
        public void Write_WithReport_CallsDebugOnTheLoggerWithReport()
        {
            const string report = "Hello!";
            var outputter = GetSubject();

            outputter.Write(report);

            _log.Received(1).Debug(report);
        }
    }
}
