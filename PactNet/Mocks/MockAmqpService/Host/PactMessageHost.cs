using System;
using System.Collections.Generic;
using System.Text;
using PactNet.Core;
using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService.Host
{
    internal class PactMessageHost : IPactMessageHost
    {
        private readonly Func<PactMessageHostConfig, IPactCoreHost> _hostFactory;
        private PactMessageHostConfig _pactMessageHostConfig;

        public PactMessageHost(Func<PactMessageHostConfig, IPactCoreHost> hostFactory)
        {
            _hostFactory = hostFactory;
        }

        public string Reify(MessageInteraction messageInteraction)
        {
            _pactCoreHost.
        }

        public void Update(MessageInteraction messageInteraction)
        {
            throw new NotImplementedException();
        }
    }
}
