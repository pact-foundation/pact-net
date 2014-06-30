using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Concord
{
    public class PactProviderRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpVerb Method { get; set; }

        public string Path { get; set; }

        //TODO: Do not change the casing for header values in the json pact file this may help https://json.codeplex.com/workitem/20923
        public Dictionary<string, string> Headers { get; set; }

        public Dictionary<string, object> Body { get; set; } //TODO: Handle different Json Formatters CamelCase or PascalCase
    }
}