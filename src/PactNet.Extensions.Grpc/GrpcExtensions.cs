using PactNet.Models;


namespace PactNet.Extensions.Grpc;

/// <summary>
/// Grpc extensions for Pact V4
/// </summary>
public static class GrpcExtensions
{
    /// <summary>
    /// Add asynchronous message (i.e. consumer/producer) interactions to the pact
    /// </summary>
    /// <param name="pact">Pact details</param>
    /// <param name="port">Port for the mock server. If null, one will be assigned automatically</param>
    /// <param name="host">Host for the mock server</param>
    /// <returns>Pact builder</returns>
    public static IGrpcPactBuilderV4 WithGrpcInteractions(this IPactV4 pact, int? port = null, IPAddress host = IPAddress.Loopback)
    {
        var pluginBuilder = pact.WithSynchronousPluginInteractions("protobuf", "0.4.0", transport: "grpc", port, host);
        var builder = new GrpcPactBuilder(pluginBuilder);
        return builder;
    }
}
