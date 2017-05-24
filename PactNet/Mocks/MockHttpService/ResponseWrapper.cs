using System.Collections.Generic;
using System.Net;

namespace PactNet.Mocks.MockHttpService
{
    internal class ResponseWrapper
    {
        public ResponseWrapper()
        {
            Contents = new byte[] { };
        }

        public byte[] Contents { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}