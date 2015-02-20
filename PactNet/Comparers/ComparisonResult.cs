using System;
using System.Collections.Generic;
using System.Linq;

namespace PactNet.Comparers
{
    public class ComparisonResult
    {
        private readonly IList<ReportedResult> _results = new List<ReportedResult>();
        public IEnumerable<ReportedResult> Results
        {
            get { return _results; }
        }

        public bool HasErrors 
        {
            get { return Results.Any(x => x.OutputType == OutputType.Error); }
        }

        public void AddInfo(string infoMessage)
        {
            _results.Add(new ReportedResult(OutputType.Info, infoMessage));
        }

        public void AddError(string errorMessage = null, object expected = null, object actual = null)
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

            _results.Add(new ReportedResult(OutputType.Error, errorMsg));
        }

        public void AddComparisonResult(ComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
            {
                return;
            }

            foreach (var result in comparisonResult.Results)
            {
                _results.Add(result);
            }
        }
    }
}