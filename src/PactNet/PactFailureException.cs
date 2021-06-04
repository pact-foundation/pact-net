using System;

namespace PactNet
{
    public class PactFailureException : Exception
    {
        public PactFailureException(string message)
            : base(message)
        {
        }
    }
}