using System.Collections.Generic;
using PactNet.Logging;
using PactNet.Logging.LogProviders;

namespace PactNet.Infrastructure.Logging
{
    internal class LocalLogProvider : LogProviderBase
    {
        private readonly LocalLogger _logger;

        public LocalLogProvider(IEnumerable<ILocalLogMessageHandler> handlers)
        {
            _logger = new LocalLogger(handlers); 
        }

        public override Logger GetLogger(string name)
        {
            return _logger.Log;
        }

        public override void Dispose()
        {
            if (_logger != null)
            {
                _logger.Dispose();
            }
        }
    }
}
