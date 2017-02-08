using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Models.ProviderService
{
    public class ProviderServicePactFile : PactFile
    {
        public ProviderServicePactFile()
            :base("1.1.0")
        {
        }

        [JsonProperty(PropertyName = "interactions")]
        public IEnumerable<ProviderServiceInteraction> Interactions { get; set; }
    }
}