using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Comparers;

namespace PactNet.Reporters
{
    public class Reporter : IReporter
    {
        private readonly IReportOutputter _outputter;

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

        //TODO: Currently only works for one level deep (this stuff needs refactoring)
        public void ReportComparisonResult(ComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
            {
                return;
            }

            Out(comparisonResult.OutputItems);
            foreach (var error in comparisonResult.Errors)
            {
                _errors.Add(error);
            }

            foreach (var result in comparisonResult.ComparisonResults)
            {
                Out(result.OutputItems);
                foreach (var error in result.Errors)
                {
                    _errors.Add(error);
                }
            }
        }

        private void Out(IEnumerable<Tuple<string, OutputType>> outputItems)
        {
            foreach (var output in outputItems)
            {
                switch (output.Item2)
                {
                    case OutputType.Error:
                        _outputter.WriteError("[Failure] {0}", output.Item1);
                        break;
                    case OutputType.Info:
                        _outputter.WriteInfo(output.Item1);
                        break;
                }
            }
        }

        public void ReportInfo(string infoMessage)
        {
            _outputter.WriteInfo(infoMessage);
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

            _outputter.WriteError("[Failure] {0}", errorMsg);
            _errors.Add(errorMsg);
        }

        public void ThrowIfAnyErrors()
        {
            if (_errors.Any())
            {
                //TODO: Take a look at BDDfy and see what they do with regards to showing errors etc
                throw new PactFailureException(String.Join(", ", _errors));
            }
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }
    }
}