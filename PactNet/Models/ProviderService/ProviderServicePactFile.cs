using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Models.ProviderService
{
    public class ProviderServicePactFile : PactFile
    {
        [JsonProperty(PropertyName = "interactions")]
        public IEnumerable<ProviderServiceInteraction> Interactions { get; set; }
    }
}