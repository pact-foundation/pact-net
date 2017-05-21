using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class DynamicBodyMapRequest
    {
        public dynamic Body { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }
}
