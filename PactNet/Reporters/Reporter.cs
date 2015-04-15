using System;
using System.Linq;
using System.Text;
using PactNet.Comparers;

namespace PactNet.Reporters
{
    internal class Reporter : IReporter
    {
        private readonly IReportOutputter _outputter;
        private int _currentTabDepth;
        private int _failureInfoCount;
        private int _failureCount;

        public Reporter(IReportOutputter outputter)
        {
            _outputter = outputter;
        }

        public Reporter() : this(
            new ConsoleReportOutputter())
        {
        }

        public void ReportInfo(string infoMessage)
        {
            _outputter.WriteInfo(infoMessage, _currentTabDepth);
        }

        public void ReportSummary(ComparisonResult comparisonResult)
        {
            WriteSummary(comparisonResult);
        }

        public void ReportFailureReasons(ComparisonResult comparisonResult)
        {
            WriteFailureReasons(comparisonResult);
        }

        public void Indent()
        {
            _currentTabDepth++;
        }

        public void ResetIndentation()
        {
            _currentTabDepth = 0;
        }

        private void WriteSummary(ComparisonResult comparisonResult, int tabDepth = 0)
        {
            if (comparisonResult == null)
            {
                return;
            }

            if (comparisonResult.HasFailure)
            {
                var failureBuilder = new StringBuilder();
                var shallowFailureCount = comparisonResult.ShallowFailureCount;

                if (shallowFailureCount > 0)
                {
                    failureBuilder.Append(" (FAILED - ");
                    for (var i = 0; i < shallowFailureCount; i++)
                    {
                        failureBuilder.Append(++_failureInfoCount);
                        if (i < shallowFailureCount - 1)
                        {
                            failureBuilder.Append(", ");
                        }
                    }
                    failureBuilder.Append(")");
                }

                _outputter.WriteError(comparisonResult.Message + failureBuilder,
                    _currentTabDepth + tabDepth);
            }
            else
            {
                _outputter.WriteSuccess(comparisonResult.Message,
                    _currentTabDepth + tabDepth);
            }

            foreach (var childComparisonResult in comparisonResult.ChildResults)
            {
                WriteSummary(childComparisonResult, tabDepth + 1);
            }
        }

        private void WriteFailureReasons(ComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
            {
                return;
            }

            if (!comparisonResult.HasFailure)
            {
                return;
            }

            _outputter.WriteInfo(Environment.NewLine + "Failures:");
            foreach (var failure in comparisonResult.Failures)
            {
                _outputter.WriteError(String.Format("{0}{1}) {2}", Environment.NewLine, ++_failureCount, failure.Result));
            }
        }
    }
}