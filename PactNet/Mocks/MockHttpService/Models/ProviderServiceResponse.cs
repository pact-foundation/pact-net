using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService.Matchers;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceResponse : IHttpMessage
    {
        private dynamic _body;

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "headers")]
        [JsonConverter(typeof(PreserveCasingDictionaryConverter))]
        public IDictionary<string, string> Headers { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "matchingRules")]
        internal IEnumerable<IMatcher> MatchingRules { get; private set; }

        [JsonProperty(PropertyName = "body")]
        public dynamic Body
        {
            get { return _body; }
            set
            {
                _body = ParseBodyMatchingRules(value);
            }
        }

        private dynamic ParseBodyMatchingRules(dynamic body)
        {
            //Find matching rules
                //Recurse through object graph
                //Need to generate the current path

            //Populate matching rules collection
            var matchingRules = new List<IMatcher>();
            matchingRules.Add(new DefaultHttpBodyMatcher(true));

            //If no matching rules can be found, we can then add the default one or apply a default one all the time.
            //Something like the law of specicifity on the path?
            MatchingRules = matchingRules;


            //Return the example value with rules stripped

            return body;
        }
    }
}