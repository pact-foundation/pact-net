using System;
using NSubstitute;
using PactNet.Comparers;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Reporters
{
    public class ReporterTests
    {
        private IReportOutputter _mockOutputter;

        private IReporter GetSubject()
        {
            _mockOutputter = Substitute.For<IReportOutputter>();

            return new Reporter(_mockOutputter);
        }

        [Fact]
        public void ReportInfo_WhenCalled_CallsWriteInfoOnOutputterWithMessage()
        {
            const string message = "Hello!";

            var reporter = GetSubject();

            reporter.ReportInfo(message);

            _mockOutputter.Received(1).WriteInfo(message);
        }

        [Fact]
        public void ReportSummary_WithFailuresOnComparisonResult_CallsWriteErrorOnOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";

            var expectedMessage = String.Format("{0} (FAILED - 1)", comparisonMessage);

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);
            comparisonResult.RecordFailure("It failed");

            reporter.ReportSummary(comparisonResult);

            _mockOutputter.Received(1).WriteError(expectedMessage, Arg.Any<int>());
        }

        [Fact]
        public void ReportSummary_WithMultipleFailuresOnComparisonResult_CallsWriteErrorOnOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";

            var expectedMessage = String.Format("{0} (FAILED - 1, 2)", comparisonMessage);

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);
            comparisonResult.RecordFailure("Failure 1");
            comparisonResult.RecordFailure("Failure 2");

            reporter.ReportSummary(comparisonResult);

            _mockOutputter.Received(1).WriteError(expectedMessage, Arg.Any<int>());
        }

        [Fact]
        public void ReportSummary_WithNoFailuresOnComparisonResult_CallsWriteSuccessOnOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);

            reporter.ReportSummary(comparisonResult);

            _mockOutputter.Received(1).WriteSuccess(comparisonMessage, Arg.Any<int>());
        }

        [Fact]
        public void ReportSummary_WithChildResultMultipleFailuresOnComparisonResult_CallsWriteErrorOnOutputterWithMessage()
        {
            const string comparisonMessage1 = "The thing I am testing";
            const string comparisonMessage2 = "The thing I am testing 2";

            var reporter = GetSubject();

            var comparisonResult2 = new ComparisonResult(comparisonMessage2);
            comparisonResult2.RecordFailure("Failure 2");

            var comparisonResult = new ComparisonResult(comparisonMessage1);
            comparisonResult.RecordFailure("Failure 1");

            comparisonResult.AddChildResult(comparisonResult2);

            reporter.ReportSummary(comparisonResult);

            _mockOutputter.Received(1).WriteError(comparisonMessage1 + " (FAILED - 1)", Arg.Any<int>());
            _mockOutputter.Received(1).WriteError(comparisonMessage2 + " (FAILED - 2)", Arg.Any<int>());
        }

        [Fact]
        public void ReportFailureReasons_WithFailuresOnComparisonResult_CallsWriteErrorOnOutputterWithFailures()
        {
            const string comparisonMessage = "The thing I am testing";
            const string comparisonFailureMessage1 = "It failed 1";
            const string comparisonFailureMessage2 = "It failed 2";


            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);
            comparisonResult.RecordFailure(comparisonFailureMessage1);
            comparisonResult.RecordFailure(comparisonFailureMessage2);

            reporter.ReportFailureReasons(comparisonResult);

            _mockOutputter.Received(1).WriteError(Environment.NewLine + "1) " + comparisonFailureMessage1, Arg.Any<int>());
            _mockOutputter.Received(1).WriteError(Environment.NewLine + "2) " + comparisonFailureMessage2, Arg.Any<int>());
        }

        [Fact]
        public void ReportFailureReasons_WithNoFailuresOnComparisonResult_DoesNotCallTheOutputter()
        {
            const string comparisonMessage = "The thing I am testing";

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);

            reporter.ReportFailureReasons(comparisonResult);

            _mockOutputter.DidNotReceive().WriteInfo(Arg.Any<String>(), Arg.Any<int>());
            _mockOutputter.DidNotReceive().WriteError(Arg.Any<String>(), Arg.Any<int>());
        }
    }
}
