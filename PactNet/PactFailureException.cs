using System;

namespace PactNet
{
    internal class PactFailureException : Exception
    {
        public PactFailureException(string message)
            : base(message)
        {
        }
    }
}