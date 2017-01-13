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
        private int _addLoggerRetryCount;

        private readonly IDictionary<string, LocalLogger> _loggers = new Dictionary<string, LocalLogger>();

        public override string AddLogger(string logDir, string loggerNameSeed, string logFileNameTemplate = "{0}.log")
        {
            var loggerName = GenerateUniqueLoggerName(loggerNameSeed);

            var logFileName = string.Format(logFileNameTemplate, loggerName);
            var logFilePath = Path.Combine(logDir, logFileName);

            lock (_sync)
            {
                try
                {
                    _loggers.Add(loggerName, new LocalLogger(Path.GetFullPath(logFilePath)));
                }
                catch (IOException)
                {
                    if (_addLoggerRetryCount > 2)
                    {
                        throw;
                    }

                    _addLoggerRetryCount++;
                    return AddLogger(logDir, loggerNameSeed + "_" + Guid.NewGuid().ToString("N"), logFileNameTemplate);
                }
            }

            return loggerName;
        }

        public override Logger GetLogger(string name)
        {
            lock (_sync)
            {
                if (!string.IsNullOrEmpty(name) &&
                    _loggers.ContainsKey(name))
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
                    loggerName = $"{nameSeed}.{++count}";
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

            return string.Empty;
        }
    }
}