using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactFile
    {
        [JsonProperty(Order = -3, PropertyName = "provider")]
        public Party Provider { get; set; }

        [JsonProperty(Order = -2, PropertyName = "consumer")]
        public Party Consumer { get; set; }

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