using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService.Matchers;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceResponse : IHttpMessage
    {
        private bool _bodyWasSet;
        private dynamic _body;

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "headers")]
        [JsonConverter(typeof(PreserveCasingDictionaryConverter))]
        public IDictionary<string, string> Headers { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "matchingRules")]
        internal IDictionary<string, IMatcher> MatchingRules { get; private set; }

        [JsonProperty(PropertyName = "body", NullValueHandling = NullValueHandling.Include)]
        public dynamic Body
        {
            get { return _body; }
            set
            {
                _bodyWasSet = true;
                _body = ParseBodyMatchingRules(value);
            }
        }

        // A not so well known feature in JSON.Net to do conditional serialization at runtime
        public bool ShouldSerializeBody()
        {
            return _bodyWasSet;
        }

        private dynamic ParseBodyMatchingRules(dynamic body)
        {
            MatchingRules = new Dictionary<string, IMatcher>
            {
                { DefaultHttpBodyMatcher.Path, new DefaultHttpBodyMatcher(new JValueMatcher(), true) }
            };

            return body;
        }
    }
}