using System.Net.Http.Headers;

namespace PactNet.Mocks.MockHttpService.Models
{
    internal class BinaryContent
    {
        public byte[] Content { get; set; }

        public MediaTypeHeaderValue ContentType { get; set; }
    }
}