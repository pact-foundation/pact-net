using System;
using PactNet.Logging;

namespace PactNet.Infrastructure.Logging
{
    internal class LocalLogMessage
    {
        public DateTime DateTime { get; private set; }
        public string DateTimeFormatted => DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
        public LogLevel Level { get; private set; }
        public Func<string> MessagePredicate { get; private set; }
        public Exception Exception { get; private set; }
        public object[] FormatParameters { get; private set; }

        public LocalLogMessage(
            LogLevel level, 
            Func<string> messagePredicate,
            Exception exception,
            object[] formatParameters)
        {
            DateTime = DateTime.Now;
            Level = level;
            MessagePredicate = messagePredicate;
            Exception = exception;
            FormatParameters = formatParameters;
        }
    }
}