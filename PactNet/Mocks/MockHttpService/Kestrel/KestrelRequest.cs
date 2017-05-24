#if USE_KESTREL

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PactNet.Mocks.MockHttpService.Kestrel
{
    internal class KestrelRequest : IRequestWrapper
    {
        private KestrelRequest()
        {
        }

        public byte[] Body { get; private set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }
        public string Method { get; private set; }
        public string Path { get; private set; }
        public string Query { get; private set; }

        public static async Task<IRequestWrapper> Create(HttpRequest request)
        {
            byte[] body;

            using (var requestBodyStream = new MemoryStream())
            {
                await request.Body.CopyToAsync(requestBodyStream);
                body = requestBodyStream.ToArray();
            }

            return new KestrelRequest
            {
                Body = body,
                Headers = request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable()),
                Method = request.Method,
                Path = request.Path,
                Query = request.QueryString.Value
            };
        }
    }
}

#endif