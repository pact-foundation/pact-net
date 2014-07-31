using System.Collections.Generic;
using PactNet.Models;

namespace PactNet.Mocks
{
    public interface IMockProvider<out TMockProviderInterface> where TMockProviderInterface : IMockProvider<TMockProviderInterface>
    {
        TMockProviderInterface Given(string providerState);
        TMockProviderInterface UponReceiving(string description);
        IEnumerable<Interaction> Interactions { get; }
    }
}