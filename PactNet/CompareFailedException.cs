using System;

namespace PactNet
{
    public class CompareFailedException : Exception
    {
        public CompareFailedException(string message)
            : base(message)
        {
        }
    }
}