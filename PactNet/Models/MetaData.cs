using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Models
{
    public class MetaData
    {
        public MetaData()
        {
            PactSpecification = new PactSpecification();
            PactNet = new pactNet();
        }

        [JsonProperty(PropertyName = "pact-specification")]
        public PactSpecification PactSpecification { get; set; }
        [JsonProperty(PropertyName = "pact-net")]
        public pactNet PactNet { get; set; }
    }
}
