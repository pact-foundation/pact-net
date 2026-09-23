using System;
using System.Collections.Generic;

namespace PactNet.Extensions.Grpc;

/// <summary>
/// Grpc request builder
/// </summary>
public interface IGrpcRequestBuilderV4
{
    /// <summary>
    /// Add a provider state
    /// </summary>
    /// <param name="providerState">Provider state description</param>
    /// <returns>Fluent builder</returns>
    IGrpcRequestBuilderV4 Given(string providerState);

    /// <summary>
    /// Add a provider state with a parameter to the interaction
    /// </summary>
    /// <param name="description">Provider state description</param>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Parameter value</param>
    /// <returns>Fluent builder</returns>
    IGrpcRequestBuilderV4 Given(string description, string name, string value);

    /// <summary>
    /// Define the response to this request
    /// </summary>
    /// <returns>Response builder</returns>
    IGrpcResponseBuilderV4 WillRespond();

    /// <summary>
    /// Configure grpc request
    /// </summary>
    /// <param name="protoFilePath">Path to the .proto file describing the service</param>
    /// <param name="serviceName">Service name as declared in the .proto file</param>
    /// <param name="methodName">Method name as declared in the .proto file</param>
    /// <param name="body">Request body, serialised to JSON for the protobuf plugin</param>
    /// <returns>Fluent builder</returns>
    IGrpcRequestBuilderV4 WithRequest(string protoFilePath, string serviceName, string methodName, dynamic body);
}

/// <summary>
/// Grpc request builder. Accumulates the protobuf plugin contents until the response body
/// completes the interaction.
/// </summary>
internal class GrpcRequestBuilder(ISynchronousPluginRequestBuilderV4 requestBuilder) : IGrpcRequestBuilderV4
{
    private const string PactProtoKey = "pact:proto";
    private const string PactProtoServiceKey = "pact:proto-service";
    private const string RequestKey = "request";
    private const string PactContentType = "pact:content-type";
    private bool requestConfigured;

    internal readonly Dictionary<string, object> InteractionContents = new();

    /// <summary>
    /// <inheritdoc cref="IGrpcRequestBuilderV4.Given(string)"/>
    /// </summary>
    public IGrpcRequestBuilderV4 Given(string providerState)
    {
        requestBuilder.Given(providerState);
        return this;
    }

    /// <summary>
    /// <inheritdoc cref="IGrpcRequestBuilderV4.Given(string, string, string)"/>
    /// </summary>
    public IGrpcRequestBuilderV4 Given(string description, string name, string value)
    {
        requestBuilder.Given(description, name, value);
        return this;
    }

    /// <summary>
    /// <inheritdoc cref="IGrpcRequestBuilderV4.WithRequest"/>
    /// </summary>
    public IGrpcRequestBuilderV4 WithRequest(string protoFilePath, string serviceName, string methodName, dynamic body)
    {
        if (this.requestConfigured)
        {
            throw new InvalidOperationException("Request has already been configured.");
        }

        this.InteractionContents.Add(PactProtoKey, protoFilePath);
        this.InteractionContents.Add(PactProtoServiceKey, $"{serviceName}/{methodName}");
        this.InteractionContents.Add(PactContentType, "application/protobuf");
        this.InteractionContents.Add(RequestKey, body);
        this.requestConfigured = true;
        return this;
    }

    /// <summary>
    /// <inheritdoc cref="IGrpcRequestBuilderV4.WillRespond"/>
    /// </summary>
    public IGrpcResponseBuilderV4 WillRespond()
    {
        if (!this.requestConfigured)
        {
            throw new InvalidOperationException("You must configure the request before defining the response");
        }

        return new GrpcResponseBuilder(requestBuilder, this.InteractionContents);
    }
}
