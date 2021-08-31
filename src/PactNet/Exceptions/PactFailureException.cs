using System;

namespace PactNet.Exceptions
{
    public class PactFailureException : Exception
    {
        public PactFailureException(string message)
            : base(message)
        {
        }
    }
}
