using System;
using System.Collections.Generic;
using NSubstitute;
using PactNet.Comparers;
using PactNet.Reporters;
using PactNet.Reporters.Outputters;
using Xunit;

namespace PactNet.Tests.Reporters
{
    public class ReporterTests
    {
        private IReportOutputter _reportOutputter;

        private IReporter GetSubject()
        {
            _reportOutputter = Substitute.For<IReportOutputter>();

            return new Reporter(new List<IReportOutputter> { _reportOutputter });
        }

        [Fact]
        public void Flush_WithReportedInfo_CallsOutputterWithEmptyString()
        {
            const string message = "Hello!";

            var reporter = GetSubject();
            reporter.ReportInfo(message);
            
            reporter.Flush();

            _reportOutputter.Received(1).Write(message);
        }

        [Fact]
        public void Flush_WithAReportedSummaryThatContainsFailuresOnTheComparisonResult_CallsOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";

            var expectedMessage = String.Format("{0} (FAILED - 1)", comparisonMessage);

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure("It failed"));
            
            reporter.ReportSummary(comparisonResult);
            reporter.Flush();

            _reportOutputter.Received(1).Write(expectedMessage);
        }
        
        [Fact]
        public void Flush_WithAReportedSummaryThatContainsMultipleFailuresOnTheComparisonResult_CallsOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";

            var expectedMessage = String.Format("{0} (FAILED - 1, 2)", comparisonMessage);

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure("Failure 1"));
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure("Failure 2"));

            reporter.ReportSummary(comparisonResult);
            reporter.Flush();

            _reportOutputter.Received(1).Write(expectedMessage);
        }

        [Fact]
        public void Flush_WithWithAReportedSummaryThatContainsNoFailuresOnTheComparisonResult_CallsOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);

            reporter.ReportSummary(comparisonResult);
            reporter.Flush();

            _reportOutputter.Received(1).Write(comparisonMessage);
        }

        [Fact]
        public void Flush_WithAReportedSummaryThatContainsAChildResultWithMultipleFailuresOnTheComparisonResult_CallsOutputterWithMessage()
        {
            const string comparisonMessage1 = "The thing I am testing";
            const string comparisonMessage2 = "The thing I am testing 2";

            var reporter = GetSubject();

            var comparisonResult2 = new ComparisonResult(comparisonMessage2);
            comparisonResult2.RecordFailure(new ErrorMessageComparisonFailure("Failure 2"));

            var comparisonResult = new ComparisonResult(comparisonMessage1);
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure("Failure 1"));

            comparisonResult.AddChildResult(comparisonResult2);

            reporter.ReportSummary(comparisonResult);
            reporter.Flush();

            _reportOutputter.Received(1).Write(Arg.Is<string>(x => x.Contains(comparisonMessage1 + " (FAILED - 1)") && x.Contains(comparisonMessage2 + " (FAILED - 2)")));
        }

        [Fact]
        public void Flush_WithReportedFailureReasonThatContainsFailuresOnTheComparisonResult_CallsOutputterWithMessage()
        {
            const string comparisonMessage = "The thing I am testing";
            const string comparisonFailureMessage1 = "It failed 1";
            const string comparisonFailureMessage2 = "It failed 2";


            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure(comparisonFailureMessage1));
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure(comparisonFailureMessage2));

            reporter.ReportFailureReasons(comparisonResult);
            reporter.Flush();

            _reportOutputter.Received(1).Write(Arg.Is<string>(x => x.Contains("1) " + comparisonFailureMessage1) && x.Contains("2) " + comparisonFailureMessage2)));
        }

        [Fact]
        public void Flush_WithReportedFailureReasonThatContainsNoFailuresOnTheComparisonResult_DoesNotCallTheOutputter()
        {
            const string comparisonMessage = "The thing I am testing";

            var reporter = GetSubject();

            var comparisonResult = new ComparisonResult(comparisonMessage);

            reporter.ReportFailureReasons(comparisonResult);
            reporter.Flush();

            _reportOutputter.DidNotReceive().Write(Arg.Any<string>());
        }
    }
}
