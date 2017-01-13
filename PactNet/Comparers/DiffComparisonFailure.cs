using System;

namespace PactNet.Comparers
{
    internal class DiffComparisonFailure : ComparisonFailure
    {
        public DiffComparisonFailure(object expected, object actual)
        {
            Result = $"Expected: {expected ?? "null"}, Actual: {actual ?? "null"}";
        }
    }
}