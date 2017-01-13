using System;
using System.Linq;
using PactNet.Logging;

namespace PactNet.Infrastructure.Logging
{
    internal class LocalLogger : IDisposable
    {
        internal string LogPath => _logHandler.LogPath;

        private readonly ILocalLogMessageHandler _logHandler;

        public LocalLogger(string logFilePath)
        {
            _logHandler = new LocalRollingLogFileMessageHandler(logFilePath);
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception, params object[] formatParameters)
        {
            //Handles the log level enabled checks
            if (messageFunc == null && 
                exception == null &&
                (formatParameters == null || !formatParameters.Any()))
            {
                return true;
            }

             _logHandler.Handle(new LocalLogMessage(logLevel, messageFunc, exception, formatParameters));

            return true;
        }

        public void Dispose()
        {
            _logHandler?.Dispose();
        }
    }
}