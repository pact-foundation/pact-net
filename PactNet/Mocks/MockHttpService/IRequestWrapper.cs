using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService
{
    internal interface IRequestWrapper
    {
        byte[] Body { get; }
        IDictionary<string, IEnumerable<string>> Headers { get; }
        string Method { get; }
        string Path { get; }
        string Query { get; }
    }
}