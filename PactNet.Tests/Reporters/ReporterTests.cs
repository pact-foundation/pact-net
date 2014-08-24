using NSubstitute;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Reporters
{
    public class ReporterTests
    {
        [Fact]
        public void ReportInfo_WhenCalled_CallsWriteInfoOnOutputterWithMessage()
        {
            const string message = "Hello!";
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportInfo(message);

            mockOutputter.Received(1).WriteInfo(message);
        }

        [Fact]
        public void ReportError_WithNoParameters_ErrorIsAdded()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError();

            Assert.NotEmpty(reporter.Errors);
        }

        [Fact]
        public void ReportError_WithNoParameters_CallsWriteErrorOnOutputter()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError();

            mockOutputter.Received(1).WriteError(Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void ReportError_WithAllParameters_ErrorIsAdded()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError("message", new { test = "" }, new { test = "tester" });

            Assert.NotEmpty(reporter.Errors);
        }

        [Fact]
        public void ReportError_WithAllParameters_CallsWriteErrorOnOutputter()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError("message", new { test = "" }, new { test = "tester" });

            mockOutputter.Received(1).WriteError(Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void ThrowIfAnyErrors_WithNoErrors_DoesNotThrow()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ThrowIfAnyErrors();
        }

        [Fact]
        public void ThrowIfAnyErrors_WithErrors_ThrowsCompareFailedException()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError("something broke");

            Assert.Throws<CompareFailedException>(() => reporter.ThrowIfAnyErrors());
        }

        [Fact]
        public void ClearErrors_WithNoErrors_ErrorsIsEmpty()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ClearErrors();

            Assert.Empty(reporter.Errors);
        }

        [Fact]
        public void ClearErrors_WithErrors_ErrorsIsEmpty()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError("something broke");
            reporter.ReportError("something broke 2");

            reporter.ClearErrors();

            Assert.Empty(reporter.Errors);
        }
    }
}
