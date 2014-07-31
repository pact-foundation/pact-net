using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServicePactFile : PactFile
    {
        //TODO: Maybe order in a predictable way, so that the file doesn't always change in git?
        [JsonProperty(PropertyName = "interactions")]
        public IEnumerable<ProviderServiceInteraction> Interactions { get; set; }
    }
}