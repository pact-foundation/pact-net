using PactNet.Interop;
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
        pact.Config.LogLevel.InitialiseLogging();
        PactHandle grpcPact = NewGrpcPact(pact.Consumer, pact.Provider);

        var builder = new GrpcPactBuilder(grpcPact, pact.Config);
        return builder;
    }

    private static PactHandle NewGrpcPact(string consumerName, string providerName)
    {
        PactHandle pact = PactInterop.NewPact(consumerName, providerName);
        PactInterop.WithSpecification(pact, PactSpecification.V4).ThrowExceptionOnFailure();
        return pact;
    }
}
