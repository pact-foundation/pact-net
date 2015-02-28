using System.Linq;
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

            mockOutputter.Received(1).WriteError(Arg.Is<string>(x => x == "[Failure] "), 0);
        }

        [Fact]
        public void ReportError_WithSomeParameters_CallsWriteErrorOnOutputter()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError(expected: "tester");

            mockOutputter.Received(1).WriteError(Arg.Is<string>(x => x == "[Failure]  Expected: tester, Actual: null"), 0);
        }

        [Fact]
        public void ReportError_WithOnlyErrorMessageParameter_ErrorIsAdded()
        {
            var errorMessage = "My error message about something that went wrong";
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError(errorMessage);

            Assert.NotEmpty(reporter.Errors);
            Assert.Equal(errorMessage, reporter.Errors.First());
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

            mockOutputter.Received(1).WriteError(Arg.Any<string>());
        }

        [Fact]
        public void ThrowIfAnyErrors_WithNoErrors_DoesNotThrow()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ThrowIfAnyErrors();
        }

        [Fact]
        public void ThrowIfAnyErrors_WithErrors_ThrowsPactFailureException()
        {
            var mockOutputter = Substitute.For<IReportOutputter>();
            var reporter = new Reporter(mockOutputter);

            reporter.ReportError("something broke");

            Assert.Throws<PactFailureException>(() => reporter.ThrowIfAnyErrors());
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
