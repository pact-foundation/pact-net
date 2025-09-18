using System.Collections.Generic;

namespace PactNet.Extensions.Grpc;

/// <summary>
/// Grpc response builder.
/// </summary>
public interface IGrpcResponseBuilderV4
{
    /// <summary>
    /// Defines the response body as a dynamic object to be serialized as json.
    /// </summary>
    /// <param name="body">Response body</param>
    void WithBody(dynamic body);
}

internal class GrpcResponseBuilder(
    Dictionary<string, object> interactionContents) : IGrpcResponseBuilderV4
{

    public void WithBody(dynamic body)
    {
        interactionContents.Add("response", body);
    }
}
