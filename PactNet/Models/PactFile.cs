using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactFile : PactDetails
    {
        [JsonProperty(PropertyName = "metadata")]
        public PactFileMetaData Metadata { get; } = new PactFileMetaData();

        public class PactFileMetaData
        {
            [JsonProperty(PropertyName = "pactSpecificationVersion")]
            public string PactSpecificationVersion { get; } = "1.1.0";
        }
    }
}