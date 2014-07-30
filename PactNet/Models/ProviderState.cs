using System;

namespace PactNet.Models
{
    public class ProviderState
    {
        public Action SetUp { get; private set; }
        public Action TearDown { get; private set; }

        public string ProviderStateDescription { get; private set; }

        public ProviderState(string providerState, Action setUp = null, Action tearDown = null)
        {
            ProviderStateDescription = providerState;
            SetUp = setUp;
            TearDown = tearDown;
        }
    }
}