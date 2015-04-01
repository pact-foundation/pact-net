using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServicePactFile : PactFile
    {
        [JsonProperty(PropertyName = "interactions")]
        public IEnumerable<ProviderServiceInteraction> Interactions { get; set; }
    }
}