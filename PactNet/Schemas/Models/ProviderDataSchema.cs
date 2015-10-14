using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using PactNet.Models;

namespace PactNet.Schemas.Models
{
    public class ProviderDataSchema : Interaction
    {
        [JsonProperty(PropertyName = "schema")]
        public JSchema Schema { get; set; }
    }
}
