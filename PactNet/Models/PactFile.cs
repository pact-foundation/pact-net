using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactFile : PactDetails
    {
        [JsonProperty(PropertyName = "metadata")]
        public dynamic Metadata { get; private set; }

        public PactFile()
        {
            Metadata = new
            {
                pactSpecificationVersion = "1.1.0"
            };
        }
    }
}