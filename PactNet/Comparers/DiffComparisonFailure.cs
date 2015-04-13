using System;

namespace PactNet.Comparers
{
    internal class DiffComparisonFailure : ComparisonFailure
    {
        public DiffComparisonFailure(object expected, object actual)
        {
            Result = String.Format("Expected: {0}, Actual: {1}", expected ?? "null", actual ?? "null");
        }
    }
}