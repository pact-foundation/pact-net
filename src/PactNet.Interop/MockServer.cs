using System;
using PactNet.Drivers;

namespace PactNet.Interop;

public static class MockServer
{
    /// <summary>
    /// Create the mock server for the current pact
    /// </summary>
    /// <param name="pact">The pact handle</param>
    /// <param name="host">Host for the mock server</param>
    /// <param name="port">Port for the mock server, or null to allocate one automatically</param>
    /// <param name="transport">Transport type for the mock server</param>
    /// <param name="tls">Enable TLS</param>
    /// <returns>Mock server port</returns>
    /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
    public static IMockServerDriver CreateMockServer(PactHandle pact, string host, int? port, string transport, bool tls)
    {
        int result = MockServerInterop.CreateMockServerForTransport(pact, host, (ushort)port.GetValueOrDefault(0), transport, null);

        if (result > 0)
        {
            return new MockServerDriver(host, result, tls);
        }

        throw result switch
        {
            -1 => new InvalidOperationException("Invalid handle when starting mock server"),
            -3 => new InvalidOperationException("Unable to start mock server"),
            -4 => new InvalidOperationException("The pact reference library panicked"),
            -5 => new InvalidOperationException("The IPAddress is invalid"),
            -6 => new InvalidOperationException("Could not create the TLS configuration with the self-signed certificate"),
            _ => new InvalidOperationException($"Unknown mock server error: {result}")
        };
    }
}
