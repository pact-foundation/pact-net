using Newtonsoft.Json;
using System.Reflection;

namespace PactNet.Models
{
    public class PactFile : PactDetails
    {
       

        [JsonProperty(PropertyName = "metadata")]
        public dynamic Metadata { get; private set; }

        public PactFile()
        {
            MetaData metaData = new MetaData()
            {
                PactSpecification = new PactSpecification() { Version = "3.0.0" },
                PactNet = new pactNet() { Version = Assembly.GetExecutingAssembly().GetName().Version.ToString() }
            };

            Metadata = metaData;
        }
    }
}