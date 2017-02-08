using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Models
{
    public class MetaData
    {
        public MetaData()
        {
            PactSpecification = new PactSpecification();
            PactNet = new PactNet();
        }

        [JsonProperty(PropertyName = "pact-specification")]
        public PactSpecification PactSpecification { get; set; }
        [JsonProperty(PropertyName = "pact-net")]
        public PactNet PactNet { get; set; }
    }
}
