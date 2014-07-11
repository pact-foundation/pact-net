using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactParty
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}