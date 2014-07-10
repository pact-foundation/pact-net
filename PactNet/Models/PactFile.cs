using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactFile
    {
        [JsonProperty(Order = -3)]
        public PactParty Provider { get; set; }

        [JsonProperty(Order = -2)]
        public PactParty Consumer { get; set; }

        public dynamic Metadata { get; private set; }

        public PactFile()
        {
            Metadata = new
            {
                PactSpecificationVersion = "1.0.0"
            };
        }
    }
}