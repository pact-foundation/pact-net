using System.Collections.Generic;

namespace Concord
{
    public class PactProviderResponse
    {
        public int Status { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public dynamic Body { get; set; }
    }
}