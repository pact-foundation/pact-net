using System;
using System.Collections.Generic;

namespace PactNet.Extensions.Grpc;

/// <summary>
/// Grpc response builder.
/// </summary>
public interface IGrpcResponseBuilderV4
{
    /// <summary>
    /// Defines the response body as a dynamic object to be serialized as json.
    /// This completes the interaction and must be the last call.
    /// </summary>
    /// <param name="body">Response body</param>
    void WithBody(dynamic body);
}

/// <summary>
/// Grpc response builder. Setting the body hands the completed contents to the plugin.
/// </summary>
internal class GrpcResponseBuilder(
    ISynchronousPluginRequestBuilderV4 requestBuilder,
    Dictionary<string, object> interactionContents) : IGrpcResponseBuilderV4
{
    private const string ResponseKey = "response";
    private const string GrpcContentType = "application/grpc";
    private bool responseConfigured;

    /// <summary>
    /// <inheritdoc cref="IGrpcResponseBuilderV4.WithBody"/>
    /// </summary>
    public void WithBody(dynamic body)
    {
        if (this.responseConfigured)
        {
            throw new InvalidOperationException("Response has already been configured.");
        }

        interactionContents.Add(ResponseKey, body);
        this.responseConfigured = true;

        requestBuilder.WithContent(GrpcContentType, interactionContents);
    }
}
