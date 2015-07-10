using System;
using System.Collections.Generic;
using System.IO;
using PactNet.Logging;
using PactNet.Logging.LogProviders;

namespace PactNet.Infrastructure.Logging
{
    internal class LocalLogProvider : LogProviderBase
    {
        private readonly object _sync = new object();

        private readonly IDictionary<string, LocalLogger> _loggers = new Dictionary<string, LocalLogger>();

        public override string AddLogger(string logDir, string loggerNameSeed, string logFileNameTemplate = "{0}.log")
        {
            var loggerName = GenerateUniqueLoggerName(loggerNameSeed);

            var logFileName = String.Format(logFileNameTemplate, loggerName);
            var logFilePath = Path.Combine(logDir, logFileName);

            lock (_sync)
            {
                _loggers.Add(loggerName, new LocalLogger(Path.GetFullPath(logFilePath)));
            }

            return loggerName;
        }

        public override Logger GetLogger(string name)
        {
            lock (_sync)
            {
                if (_loggers.ContainsKey(name))
                {
                    return _loggers[name].Log;
                }
            }
            return LogProvider.NoOpLogger.Instance.Log;
        }

        public override void RemoveLogger(string name)
        {
            lock (_sync)
            {
                if (_loggers.ContainsKey(name))
                {
                    _loggers[name].Dispose();
                    _loggers.Remove(name);
                }
            }
        }

        private string GenerateUniqueLoggerName(string nameSeed)
        {
            var count = 0;
            var loggerName = nameSeed;
            lock (_sync)
            {
                while (_loggers.ContainsKey(loggerName))
                {
                    loggerName = String.Format("{0}.{1}", nameSeed, ++count);
                }
            }

            return loggerName;
        }

        public override string ResolveLogPath(string name)
        {
            lock (_sync)
            {
                if (_loggers.ContainsKey(name))
                {
                    return _loggers[name].LogPath;
                }
            }

            return String.Empty;
        }
    }
}