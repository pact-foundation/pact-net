using Newtonsoft.Json;
using System.Reflection;

namespace PactNet.Models
{
    public class PactFile : PactDetails
    {
        [JsonProperty(PropertyName = "metadata")]
        public dynamic Metadata { get; private set; }

        public PactFile(string version)
        {
            MetaData metaData = new MetaData()
            {
                PactSpecification = new PactSpecification() { Version = version },
                PactNet = new PactNet() { Version = Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            };

            Metadata = metaData;
        }
    }
}