using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Comparers;

namespace PactNet.Reporters
{
    internal class Reporter : IReporter
    {
        private readonly IEnumerable<Action<string>> _outputters;

        private int _currentTabDepth;
        private int _failureInfoCount;
        private int _failureCount;

        private readonly IList<string> _reportLines = new List<string>();

        internal Reporter(IEnumerable<Action<string>> outputters)
        {
            _outputters = outputters;
        }

        public Reporter(PactVerifierConfig config)
            : this(new List<Action<string>>
            {
                Console.WriteLine, 
                new FileReportOutputter(config.LoggerName).Write
            })
        {
        }

        public void ReportInfo(string infoMessage)
        {
            AddReportLine(infoMessage, _currentTabDepth);
        }

        public void ReportSummary(ComparisonResult comparisonResult)
        {
            AddSummary(comparisonResult);
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

        public void Flush()
        {
            if (!_reportLines.Any())
            {
                return;
            }

            foreach (var outputter in _outputters)
            {
                outputter(String.Join(Environment.NewLine, _reportLines));
            }
        }

        private void AddSummary(ComparisonResult comparisonResult, int tabDepth = 0)
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

                AddReportLine(comparisonResult.Message + failureBuilder, _currentTabDepth + tabDepth);
            }
            else
            {
                AddReportLine(comparisonResult.Message, _currentTabDepth + tabDepth);
            }

            foreach (var childComparisonResult in comparisonResult.ChildResults)
            {
                AddSummary(childComparisonResult, tabDepth + 1);
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

            AddReportLine(String.Empty, 0);
            AddReportLine("Failures:", 0);

            foreach (var failure in comparisonResult.Failures)
            {
                AddReportLine(String.Empty, 0);
                AddReportLine(String.Format("{0}) {1}", ++_failureCount, failure.Result), 0);
            }
        }

        private void AddReportLine(string message, int tabDepth)
        {
            var indentation = new String(' ', tabDepth*2); //Each tab we want to be 2 space chars
            _reportLines.Add(indentation + message);
        }
    }
}