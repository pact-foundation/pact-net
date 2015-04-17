using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Logging;

namespace PactNet.Infrastructure.Logging
{
    internal class LocalLogger : IDisposable
    {
        private readonly IEnumerable<ILocalLogMessageHandler> _logHandlers;

        public LocalLogger(IEnumerable<ILocalLogMessageHandler> logHandlers)
        {
            _logHandlers = logHandlers;
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

            foreach (var handler in _logHandlers)
            {
                handler.Handle(new LocalLogMessage(logLevel, messageFunc, exception, formatParameters));
            }

            return true;
        }

        public void Dispose()
        {
            foreach (var handler in _logHandlers.Where(handler => handler != null))
            {
                handler.Dispose();
            }
        }
    }
}