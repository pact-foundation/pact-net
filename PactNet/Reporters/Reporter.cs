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

        public void ReportComparisonResult(ComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
            {
                return;
            }

            Out(comparisonResult.Results);
            foreach (var error in comparisonResult.Results.Where(x => x.OutputType == OutputType.Error))
            {
                _errors.Add(error.Message);
            }
        }

        private void Out(IEnumerable<ReportedResult> results)
        {
            foreach (var result in results)
            {
                switch (result.OutputType)
                {
                    case OutputType.Error:
                        _outputter.WriteError("[Failure] {0}", result.Message);
                        break;
                    case OutputType.Info:
                        _outputter.WriteInfo(result.Message);
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