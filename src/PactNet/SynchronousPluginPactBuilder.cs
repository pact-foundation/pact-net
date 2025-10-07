using PactNet.Drivers.Plugins;
using PactNet.Models;

namespace PactNet;

internal class SynchronousPluginPactBuilder(
    IPluginPactDriver pact,
    PactConfig config,
    int? port,
    IPAddress host,
    string transport = null)
    : AbstractPactBuilder(pact, config, port, host, transport), ISynchronousPluginPactBuilderV4
{
    public ISynchronousPluginRequestBuilderV4 UponReceiving(string description)
    {
        return new SynchronousPluginRequestBuilder(pact.NewSyncInteraction(description));
    }

    public void Dispose() => pact?.Dispose();
}
