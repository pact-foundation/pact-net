using System;

namespace PactNet
{
    public interface IProviderStates
    {
        IProviderStates ProviderState(string providerState, Action setUp = null, Action tearDown = null);
    }
}