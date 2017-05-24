#if USE_NANCY
using Nancy;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class NancyRequest : IRequestWrapper
    {
        private NancyRequest() { }

        public byte[] Body { get; private set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }
        public string Method { get; private set; }
        public string Path { get; private set; }
        public string Query { get; private set; }

        public static IRequestWrapper Create(Request request)
        {
            var body = new byte[request.Body.Length];

            request.Body.Read(body, 0, (int)request.Body.Length);

            return new NancyRequest
            {
                Body = body,
                Headers = request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value),
                Method = request.Method,
                Path = request.Path,
                Query = request.Url.Query
            };
        }
    }
}
#endif