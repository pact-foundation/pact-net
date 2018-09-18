using System;
using System.Collections.Generic;
using System.Text;
using PactNet.Core;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Mocks.MockAmqpService.Host
{
    public class RabbitMqProviderHostConfig : IPactCoreHostConfig
    {
        public string Script { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IEnumerable<IOutput> Outputters { get; }
        public IDictionary<string, string> Environment { get; }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
