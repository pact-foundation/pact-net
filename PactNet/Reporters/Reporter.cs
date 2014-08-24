using System;
using System.Collections.Generic;
using System.Linq;

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

        public void ReportInfo(string infoMessage)
        {
            _outputter.WriteInfo(infoMessage);
        }

        public void ReportError(string errorMessage = null, object expected = null, object actual = null)
        {
            var errorMsg = String.Format("{0} Expected: {1}, Actual: {2}", errorMessage, expected, actual);
            _outputter.WriteError("[Failure] {0}", errorMsg);
            _errors.Add(errorMsg);
        }

        public void ThrowIfAnyErrors()
        {
            if (_errors.Any())
            {
                //TODO: Take a look at BDDfy and see what they do with regards to showing errors etc
                throw new CompareFailedException(String.Join(", ", _errors));
            }
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }
    }
}