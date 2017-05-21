using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class BinaryContentMapRequest
    {
        public byte[] Content { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }
}
