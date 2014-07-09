using System;

namespace PactNet
{
    public class ComparisonFailedException : Exception
    {
        public ComparisonFailedException(string message)
            :base(String.Format("[Failure] {0}", message))
        {
            Console.WriteLine("[Failure] {0}", message);
        }

        public ComparisonFailedException(object expected, object actual)
            : this(String.Format("Expected: {0}, Actual: {1}", expected, actual))
        {
        }

        public ComparisonFailedException(string context, object expected, object actual)
            : this(String.Format("{0} Expected: {1}, Actual: {2}", context, expected, actual))
        {
        }
    }
}