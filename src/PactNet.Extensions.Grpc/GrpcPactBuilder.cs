using System;
using System.Threading.Tasks;

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

/// <summary>
/// Grpc pact v4 builder, layered over the raw synchronous plugin builder
/// </summary>
internal class GrpcPactBuilder : IGrpcPactBuilderV4
{
    private readonly ISynchronousPluginPactBuilderV4 pactBuilder;

    /// <summary>
    /// Initialises a new instance of the <see cref="GrpcPactBuilder"/> class.
    /// </summary>
    /// <param name="pactBuilder">Underlying synchronous plugin pact builder</param>
    internal GrpcPactBuilder(ISynchronousPluginPactBuilderV4 pactBuilder)
    {
        this.pactBuilder = pactBuilder;
    }

    /// <summary>
    /// Create a new request/response interaction
    /// </summary>
    /// <param name="description">Interaction description</param>
    /// <returns>Fluent builder</returns>
    public IGrpcRequestBuilderV4 UponReceiving(string description)
        => new GrpcRequestBuilder(this.pactBuilder.UponReceiving(description));

    /// <summary>
    /// <inheritdoc cref="IPactBuilder.Verify"/>
    /// </summary>
    public void Verify(Action<IConsumerContext> interact)
        => this.pactBuilder.Verify(interact);

    /// <summary>
    /// <inheritdoc cref="IPactBuilder.VerifyAsync"/>
    /// </summary>
    public Task VerifyAsync(Func<IConsumerContext, Task> interact)
        => this.pactBuilder.VerifyAsync(interact);

    public void Dispose() => this.pactBuilder.Dispose();
}
