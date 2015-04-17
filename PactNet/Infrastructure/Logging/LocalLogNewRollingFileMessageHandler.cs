using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PactNet.Infrastructure.Logging
{
    internal class LocalLogNewRollingFileMessageHandler : ILocalLogMessageHandler
    {
        private static readonly object Sync = new object();
        private readonly StreamWriter _writer;

        public LocalLogNewRollingFileMessageHandler(string filePath)
        {
            TryCreateDirectory(filePath);
            TryDeleteFile(filePath);
            var file = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(file, Encoding.UTF8);
        }

        public void Handle(LocalLogMessage logMessage)
        {
            var messageFormat = logMessage.MessagePredicate != null ?
                logMessage.MessagePredicate() :
                String.Empty;

            string message;
            if (logMessage.Exception != null)
            {
                message = String.Format("{0}. Exception: {1} - {2}", messageFormat, logMessage.Exception, logMessage.Exception.StackTrace);
            }
            else if (logMessage.FormatParameters != null && logMessage.FormatParameters.Any())
            {
                message = String.Format(messageFormat, logMessage.FormatParameters);
            }
            else
            {
                message = messageFormat;
            }
            
            lock (Sync)
            {
                _writer.WriteLine("{0} [{1}] {2}", logMessage.DateTimeFormatted, logMessage.Level, message);
                _writer.Flush();
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        private static void TryCreateDirectory(string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create log directory.", ex);
            }
        }

        private static void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not delete log file.", ex);
            }
        }
    }
}