using System.Net.Http.Headers;

namespace PactNet.Mocks.MockHttpService.Models
{
    internal class DynamicBody
    {
        public dynamic Body { get; set; }

        public MediaTypeHeaderValue ContentType { get; set; }
    }
}