using System;
using System.Collections.Generic;
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

        private readonly IList<string> _errors = new List<string>();
        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }

        public Reporter(IReportOutputter outputter)
        {
            _outputter = outputter;
        }

        public Reporter() : this(
            new ConsoleReportOutputter())
        {
        }

        public void ReportSummary(ComparisonResult comparisonResult)
        {
            WriteSummary(comparisonResult);
        }

        public void ReportFailureReasons(ComparisonResult comparisonResult)
        {
            WriteFailureReasons(comparisonResult);
        }

        private void WriteSummary(ComparisonResult comparisonResult, int tabDepth = 0)
        {
            if (comparisonResult == null)
            {
                return;
            }

            if (comparisonResult.HasFailures)
            {
                var failureBuilder = new StringBuilder();
                var shallowFailureCount = comparisonResult.ShallowFailureCount;

                if (shallowFailureCount > 0)
                {
                    failureBuilder.Append(" (FAILED - ");
                    for (var i = 0; i < shallowFailureCount; i++)
                    {
                        failureBuilder.Append(++_failureInfoCount + "");
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

            var failures = comparisonResult.Failures.ToList();
            if (!failures.Any())
            {
                return;
            }

            _outputter.WriteInfo(Environment.NewLine + "Failures:");
            foreach (var failure in failures)
            {
                _outputter.WriteError(String.Format("{0}{1}) {2}", Environment.NewLine, ++_failureCount, failure.Result));
                _errors.Add(failure.Result); //TODO: Fix this up, dont do it
            }
        }

        public void ReportInfo(string infoMessage)
        {
            _outputter.WriteInfo(infoMessage, _currentTabDepth);
        }

        public void ReportError(string errorMessage = null, object expected = null, object actual = null)
        {
            string errorMsg;
            if (expected != null || actual != null)
            {
                errorMsg = String.Format("{0} Expected: {1}, Actual: {2}", errorMessage, expected ?? "null", actual ?? "null");
            }
            else
            {
                errorMsg = errorMessage;
            }

            _outputter.WriteError(String.Format("[Failure] {0}", errorMsg), _currentTabDepth);
            _errors.Add(errorMsg);
        }

        public void ThrowIfAnyErrors()
        {
            if (_errors.Any())
            {
                //TODO: Take a look at BDDfy and see what they do with regards to showing errors etc
                throw new PactFailureException("See output for failure details.");
            }
        }

        public void ClearErrors()
        {
            _errors.Clear();
            _failureCount = 0;
            _failureInfoCount = 0;
        }

        public void Indent()
        {
            _currentTabDepth++;
        }

        public void ResetIndentation()
        {
            _currentTabDepth = 0;
        }
    }
}