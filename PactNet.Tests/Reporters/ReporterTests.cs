using NSubstitute;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Reporters
{
    public class ReporterTests
    {
        //TODO: Test the reporter

        [Fact]
        public void ReportInfo_WhenCalled_CallsWriteInfoOnOutputterWithMessage()
        {
            const string message = "Hello!";
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportInfo(message);

            mockOutputter.Received(1).WriteInfo(message);
        }
    }
}
