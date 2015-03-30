using Newtonsoft.Json;

namespace PactNet.Models
{
    internal class Party
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}