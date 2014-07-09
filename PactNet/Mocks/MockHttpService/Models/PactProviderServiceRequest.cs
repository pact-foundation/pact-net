using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactProviderServiceRequest
    {
        [JsonConverter(typeof(LowercaseStringEnumConverter))]
        public HttpVerb Method { get; set; }

        public string Path { get; set; }

        [JsonConverter(typeof(DictionaryConverter))]
        public Dictionary<string, string> Headers { get; set; }

        public dynamic Body { get; set; } //TODO: Handle different Json Formatters CamelCase or PascalCase
    }
}