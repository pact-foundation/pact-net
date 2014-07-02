using System;

namespace PactNet
{
    public class PactAssertException : Exception
    {
        public PactAssertException(string message)
            :base(String.Format("[Failure] {0}", message))
        {
        }

        public PactAssertException(object expected, object actual)
            : this(String.Format("Expected: {0}, Actual: {1}", expected, actual))
        {
        }

        public PactAssertException(string context, object expected, object actual)
            : this(String.Format("{0} Expected: {1}, Actual: {2}", context, expected, actual))
        {
        }
    }
}