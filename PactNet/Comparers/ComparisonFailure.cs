using System;

namespace PactNet.Comparers
{
    internal class ComparisonFailure
    {
        public string Result { get; private set; }

        public ComparisonFailure(string result)
        {
            Result = result;
        }

        public ComparisonFailure(object expected, object actual)
        {
            Result = String.Format("Expected: {0}, Actual: {1}", expected ?? "null", actual ?? "null");
        }
    }
}