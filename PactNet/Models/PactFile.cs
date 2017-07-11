using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Models
{
    public class PactFile : PactDetails
    {
        [JsonProperty(PropertyName = "interactions")]
        public IEnumerable<ProviderServiceInteraction> Interactions { get; set; }

        [JsonProperty(PropertyName = "metadata")]
        public dynamic Metadata { get; set; }
    }
}