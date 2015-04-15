using System;
using PactNet.Logging;

namespace PactNet
{
    public class PactFailureException : Exception
    {
        public PactFailureException(string message)
            : base(!String.IsNullOrEmpty(LogProvider.LogFilePath) ?
            String.Format("{0} See {1} for details.", message, LogProvider.LogFilePath) :
            message)
        {
        }
    }
}