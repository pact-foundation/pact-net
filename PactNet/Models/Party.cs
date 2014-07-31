using Newtonsoft.Json;

namespace PactNet.Models
{
    public class Party
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}