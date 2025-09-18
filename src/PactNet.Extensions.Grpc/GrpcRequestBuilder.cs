using System;
using System.Collections.Generic;
using PactNet.Interop;

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
    /// Add a provider state with one or more parameters
    /// </summary>
    /// <param name="providerState">Provider state description</param>
    /// <param name="parameters">Provider state parameters</param>
    /// <returns>Fluent builder</returns>
    IGrpcRequestBuilderV4 Given(string providerState, IDictionary<string, string> parameters);

    /// <summary>
    /// Define the response to this request
    /// </summary>
    /// <returns>Response builder</returns>
    IGrpcResponseBuilderV4 WillRespond();

    /// <summary>
    /// Configure grpc request
    /// </summary>
    /// <param name="protoFilePath"></param>
    /// <param name="serviceName"></param>
    /// <param name="methodName"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    IGrpcRequestBuilderV4 WithRequest(string protoFilePath, string serviceName, string methodName, dynamic body);
}

internal class GrpcRequestBuilder(InteractionHandle interaction) : IGrpcRequestBuilderV4
{
    private const string PactProtoKey = "pact:proto";
    private const string PactProtoServiceKey = "pact:proto-service";
    private const string RequestKey = "request";
    private const string PactContentType = "pact:content-type";
    private bool requestConfigured;

    internal readonly Dictionary<string, object> InteractionContents = new();

    /// <summary>
    /// Add a provider state
    /// </summary>
    /// <param name="providerState">Provider state description</param>
    /// <returns>Fluent builder</returns>
    public IGrpcRequestBuilderV4 Given(string providerState)
    {
        NativeInterop.Given(interaction, providerState).ThrowExceptionOnFailure();
        return this;
    }

    /// <summary>
    /// Add a provider state with one or more parameters
    /// </summary>
    /// <param name="providerState">Provider state description</param>
    /// <param name="parameters">Provider state parameters</param>
    /// <returns>Flient builder</returns>
    public IGrpcRequestBuilderV4 Given(string providerState, IDictionary<string, string> parameters)
    {
        foreach (var param in parameters)
        {
            NativeInterop.GivenWithParam(interaction, providerState, param.Key, param.Value).ThrowExceptionOnFailure();
        }

        return this;
    }

    /// <summary>
    /// Configure grpc request
    /// </summary>
    /// <param name="protoFilePath"></param>
    /// <param name="serviceName"></param>
    /// <param name="methodName"></param>
    /// <param name="body"></param>
    /// <returns></returns>
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
    /// Define the response to this request
    /// </summary>
    /// <returns>Response builder</returns>
    public IGrpcResponseBuilderV4 WillRespond()
    {
        if (!this.requestConfigured)
        {
            throw new InvalidOperationException("You must configure the request before defining the response");
        }

        var builder = new GrpcResponseBuilder(this.InteractionContents);
        return builder;
    }
}
