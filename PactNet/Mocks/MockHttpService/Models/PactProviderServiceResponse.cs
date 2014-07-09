using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactProviderServiceResponse
    {
        public int Status { get; set; }

        [JsonConverter(typeof(DictionaryConverter))]
        public Dictionary<string, string> Headers { get; set; }
        
        public dynamic Body { get; set; }
    }
}