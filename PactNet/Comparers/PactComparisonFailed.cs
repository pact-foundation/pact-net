using System;

namespace PactNet.Comparers
{
    public class PactComparisonFailed : Exception
    {
        public PactComparisonFailed(string message)
            :base(String.Format("[Failure] {0}", message))
        {
            Console.WriteLine("[Failure] {0}", message);
        }

        public PactComparisonFailed(object expected, object actual)
            : this(String.Format("Expected: {0}, Actual: {1}", expected, actual))
        {
        }

        public PactComparisonFailed(string context, object expected, object actual)
            : this(String.Format("{0} Expected: {1}, Actual: {2}", context, expected, actual))
        {
        }
    }
}