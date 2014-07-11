using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactFile
    {
        [JsonProperty(Order = -3, PropertyName = "provider")]
        public PactParty Provider { get; set; }

        [JsonProperty(Order = -2, PropertyName = "consumer")]
        public PactParty Consumer { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public dynamic Metadata { get; private set; }

        public PactFile()
        {
            Metadata = new
            {
                pactSpecificationVersion = "1.0.0"
            };
        }
    }
}