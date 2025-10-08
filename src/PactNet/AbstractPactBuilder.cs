using System;
using System.Threading.Tasks;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Internal;
using PactNet.Models;

namespace PactNet;

/// <summary>
/// Abstract pact builder that contains shared functionality of different types of pact interactions.
/// </summary>
public abstract class AbstractPactBuilder : IPactBuilder
{
    private readonly ICompletedPactDriver pact;
    private readonly PactConfig config;
    private readonly int? port;
    private readonly IPAddress host;
    private readonly string transport;

    /// <summary>
    /// Initialises a new instance of the <see cref="AbstractPactBuilder"/> class.
    /// </summary>
    /// <param name="pact">Pact driver</param>
    /// <param name="config">Pact config</param>
    /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
    /// <param name="host">Optional host, otherwise loopback is used</param>
    /// <param name="transport">The transport to use (i.e. http, https, grpc). Must be a valid UTF-8 NULL-terminated string, or NULL or empty, in which case http will be used.</param>
    protected AbstractPactBuilder(ICompletedPactDriver pact, PactConfig config, int? port, IPAddress host,
        string transport = null)
    {
        this.pact = pact;
        this.config = config;
        this.port = port;
        this.host = host;
        this.transport = transport;
    }

    /// <summary>
    /// Verify the configured interactions
    /// </summary>
    /// <param name="interact">Action to perform the real interactions against the mock driver</param>
    /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
    public virtual void Verify(Action<IConsumerContext> interact)
    {
        Guard.NotNull(interact, nameof(interact));

        using IMockServerDriver mockServer = this.StartMockServer();

        try
        {
            interact(new ConsumerContext { MockServerUri = mockServer.Uri });

            this.VerifyInternal(mockServer);
        }
        finally
        {
            this.PrintLogs(mockServer);
        }
    }

    /// <summary>
    /// Verify the configured interactions
    /// </summary>
    /// <param name="interact">Action to perform the real interactions against the mock driver</param>
    /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
    public virtual async Task VerifyAsync(Func<IConsumerContext, Task> interact)
    {
        Guard.NotNull(interact, nameof(interact));

        using IMockServerDriver mockServer = this.StartMockServer();

        try
        {
            await interact(new ConsumerContext { MockServerUri = mockServer.Uri });

            this.VerifyInternal(mockServer);
        }
        finally
        {
            this.PrintLogs(mockServer);
        }
    }

    /// <summary>
    /// Start the mock driver
    /// </summary>
    /// <returns>Mock driver</returns>
    private IMockServerDriver StartMockServer()
    {
        string hostIp = this.host switch
        {
            IPAddress.Loopback => "127.0.0.1",
            IPAddress.Any => "0.0.0.0",
            _ => throw new ArgumentOutOfRangeException(nameof(this.host), this.host, "Unsupported IPAddress value")
        };

        // TODO: add TLS support
        return this.pact.CreateMockServer(hostIp, this.port, false, transport);
    }

    /// <summary>
    /// Verify the interactions after the consumer client has been invoked
    /// </summary>
    /// <param name="mockServer">Mock server</param>
    private void VerifyInternal(IMockServerDriver mockServer)
    {
        string errors = mockServer.MockServerMismatches();

        if (string.IsNullOrWhiteSpace(errors) || errors == "[]")
        {
            this.pact.WritePactFile(mockServer.Port, this.config.PactDir);
            return;
        }

        this.config.WriteLine(string.Empty);
        this.config.WriteLine("Verification mismatches:");
        this.config.WriteLine(string.Empty);
        this.config.WriteLine(errors);

        throw new PactFailureException("Pact verification failed. See output for details");
    }

    /// <summary>
    /// Print logs to the configured outputs
    /// </summary>
    /// <param name="mockServer">Mock server</param>
    private void PrintLogs(IMockServerDriver mockServer)
    {
        string logs = mockServer.MockServerLogs();

        this.config.WriteLine("Mock driver logs:");
        this.config.WriteLine(string.Empty);
        this.config.WriteLine(logs);
    }
}
