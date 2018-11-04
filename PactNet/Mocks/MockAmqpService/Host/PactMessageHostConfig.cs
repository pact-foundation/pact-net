using System;
using System.Collections.Generic;
using System.Text;
using PactNet.Core;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Mocks.MockAmqpService.Host
{
    internal class PactMessageHostConfig : IPactCoreHostConfig
    {
        public string Script { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IEnumerable<IOutput> Outputters { get; }
        public IDictionary<string, string> Environment { get; }

        public PactMessageHostConfig(PactConfig pactConfig, string arguments, bool waitForExit)
        {
            Outputters = pactConfig.Outputters;
            Script = "pact-message";

            Arguments = arguments;

            WaitForExit = waitForExit;
        }
    }
}
