using System;
using System.Collections.Generic;

namespace PactNet.Comparers
{
    public class ComparisonResult
    {
        private readonly IList<string> _errors = new List<string>();
        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }

        private readonly IList<Tuple<string, OutputType>> _outputItems = new List<Tuple<string, OutputType>>();
        public IEnumerable<Tuple<string, OutputType>> OutputItems
        {
            get { return _outputItems; }
        }

        //TODO: Only one depth is supported, so come up with a better way of doing this
        private readonly IList<ComparisonResult> _comparisonResults = new List<ComparisonResult>();
        public IEnumerable<ComparisonResult> ComparisonResults
        {
            get { return _comparisonResults; }
        }

        public void AddInfo(string infoMessage)
        {
            _outputItems.Add(new Tuple<string, OutputType>(infoMessage, OutputType.Info));
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

            _outputItems.Add(new Tuple<string, OutputType>(errorMsg, OutputType.Error));
            _errors.Add(errorMsg);
        }

        public void AddComparisonResult(ComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
            {
                return;
            }

            _comparisonResults.Add(comparisonResult);
        }
    }
}