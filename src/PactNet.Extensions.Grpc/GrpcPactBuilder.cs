using System;
using System.Text.Json;
using System.Threading.Tasks;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Interop.Drivers;
using PactNet.Models;

namespace PactNet.Extensions.Grpc;


/// <summary>
/// Grpc pact v4 builder
/// </summary>
public interface IGrpcPactBuilderV4: IPactBuilder, IDisposable
{
    /// <summary>
    /// Add a new interaction to the pact
    /// </summary>
    /// <param name="description">Interaction description</param>
    /// <returns>Fluent builder</returns>
    IGrpcRequestBuilderV4 UponReceiving(string description);
}

internal class GrpcPactBuilder : IGrpcPactBuilderV4
{
    private readonly PactHandle pact;
    private readonly PactConfig config;
    private readonly int? port;
    private readonly IPAddress host;
    private readonly IPluginDriver pluginDriver;
    private InteractionHandle interaction;
    private GrpcRequestBuilder requestBuilder;
    private bool interactionInitialized;

    /// <summary>
    /// Initialises a new instance of the <see cref="GrpcPactBuilder"/> class.
    /// </summary>
    /// <param name="pact">Pact handle</param>
    /// <param name="config">Pact config</param>
    /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
    /// <param name="host">Optional host, otherwise loopback is used</param>
    internal GrpcPactBuilder(PactHandle pact, PactConfig config, int? port = null, IPAddress host = IPAddress.Loopback)
    {
        this.pact = pact;
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.port = port;
        this.host = host;
        pluginDriver = pact.UsePlugin("protobuf", "0.4.0");
    }

    /// <summary>
    /// Create a new request/response interaction
    /// </summary>
    /// <param name="description">Interaction description</param>
    /// <returns>Fluent builder</returns>
    public IGrpcRequestBuilderV4 UponReceiving(string description)
    {
        if (interactionInitialized)
        {
            throw new InvalidOperationException("An interaction has already been initialized for this pact.");
        }

        interaction = pact.NewSyncMessageInteraction(description);
        interactionInitialized = true;
        this.requestBuilder= new GrpcRequestBuilder(interaction);
        return this.requestBuilder;
    }

    /// <summary>
    /// Verify the configured interactions
    /// </summary>
    /// <param name="interact">Action to perform the real interactions against the mock driver</param>
    /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
    public void Verify(Action<IConsumerContext> interact)
    {
        if (interact == null)
        {
            throw new ArgumentNullException(nameof(interact));
        }

        this.pluginDriver.WithInteractionContents(interaction, "application/grpc", JsonSerializer.Serialize(this.requestBuilder.InteractionContents));

        using IMockServerDriver mockServer = this.StartMockServer();

        try
        {
            interact(new ConsumerContext { MockServerUri = mockServer.Uri });

            this.VerifyInternal(mockServer);
        }
        finally
        {
            this.PrintLogs();
        }
    }

    /// <summary>
    /// Verify the configured interactions
    /// </summary>
    /// <param name="interact">Action to perform the real interactions against the mock driver</param>
    /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
    public async Task VerifyAsync(Func<IConsumerContext, Task> interact)
    {
        if (interact == null)
        {
            throw new ArgumentNullException(nameof(interact));
        }

        this.pluginDriver.WithInteractionContents(interaction, "application/grpc", JsonSerializer.Serialize(this.requestBuilder.InteractionContents));

        using IMockServerDriver mockServer = this.StartMockServer();

        try
        {
            await interact(new ConsumerContext { MockServerUri = mockServer.Uri });

            this.VerifyInternal(mockServer);
        }
        finally
        {
            this.PrintLogs();
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
        return this.pact.CreateMockServer(hostIp, this.port, "grpc", false);
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
            PactFileWriter.WritePactFileForPort(mockServer.Port, this.config.PactDir);
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
    private void PrintLogs()
    {
        string logs = LoggingInterop.FetchLogBuffer(null);

        this.config.WriteLine("Mock driver logs:");
        this.config.WriteLine(string.Empty);
        this.config.WriteLine(logs);
    }

    public void Dispose() => this.pluginDriver.Dispose();
}
