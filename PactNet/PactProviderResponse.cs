using System.Collections.Generic;

namespace PactNet
{
    public class PactProviderResponse
    {
        public int Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public dynamic Body { get; set; }
    }
}