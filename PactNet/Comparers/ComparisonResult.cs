using System;
using System.Collections.Generic;
using System.Linq;

namespace PactNet.Comparers
{
    internal class ComparisonResult
    {
        public string Message { get; private set; }

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

        public bool HasFailure => Failures.Any();

        public int ShallowFailureCount => _failures.Count;

        private readonly IList<ComparisonResult> _childResults = new List<ComparisonResult>();
        public IEnumerable<ComparisonResult> ChildResults => _childResults;

        public ComparisonResult(string message = null)
        {
            Message = message;
        }

        public ComparisonResult(string messageFormat, params object[] args)
        {
            Message = string.Format(messageFormat, args);
        }

        public void RecordFailure(ComparisonFailure comparisonFailure)
        {
            _failures.Add(comparisonFailure);
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