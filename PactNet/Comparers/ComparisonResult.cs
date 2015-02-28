using System;
using System.Collections.Generic;
using System.Linq;

namespace PactNet.Comparers
{
    //TODO: Can we make this internal?
    public class ComparisonResult
    {
        public string ComparisonDescription { get; private set; }

        private readonly IList<ComparisonFailure> _failures = new List<ComparisonFailure>();
        public IEnumerable<ComparisonFailure> Failures
        {
            get
            {
                var failuresDeep = new List<ComparisonFailure>();
                GetChildComparisonResultFailures(this, failuresDeep);
                return failuresDeep;
            }
        }

        private readonly IList<ComparisonResult> _childResults = new List<ComparisonResult>();
        public IEnumerable<ComparisonResult> ChildResults
        {
            get { return _childResults; }
        }

        public bool HasFailures
        {
            get
            {
                return Failures.Any();
            }
        }

        public bool HasShallowFailure
        {
            get
            {
                return _failures.Any();
            }
        }

        public ComparisonResult(string message = null)
        {
            ComparisonDescription = message;
        }

        public ComparisonResult(string messageFormat, params object[] args)
        {
            ComparisonDescription = String.Format(messageFormat, args);
        }

        public void RecordFailure(string errorMessage = null, object expected = null, object actual = null)
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

            _failures.Add(new ComparisonFailure(errorMsg));
        }

        public void AddChildResult(ComparisonResult comparisonResult)
        {
            if (comparisonResult == null)
            {
                return;
            }

            _childResults.Add(comparisonResult);
        }

        private static void GetChildComparisonResultFailures(ComparisonResult comparisonResult, List<ComparisonFailure> fails)
        {
            fails.AddRange(comparisonResult._failures);
            foreach (var childComparisonResult in comparisonResult.ChildResults)
            {
                GetChildComparisonResultFailures(childComparisonResult, fails);
            }
        }
    }
}