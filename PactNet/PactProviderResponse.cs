using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;

namespace PactNet
{
    public class PactProviderResponse
    {
        public int Status { get; set; }

        [JsonConverter(typeof(DictionaryConverter))]
        public Dictionary<string, string> Headers { get; set; }
        
        public dynamic Body { get; set; }
    }
}