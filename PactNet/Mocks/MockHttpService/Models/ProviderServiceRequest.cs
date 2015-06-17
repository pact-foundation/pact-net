using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceRequest : IHttpMessage
    {
        private dynamic _body;

        [JsonProperty(PropertyName = "method")]
        [JsonConverter(typeof(CamelCaseStringEnumConverter))]
        public HttpVerb Method { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "headers")]
        [JsonConverter(typeof(PreserveCasingDictionaryConverter))]
        public IDictionary<string, string> Headers { get; set; }

        [JsonIgnore]
        [JsonProperty(PropertyName = "matchingRules")]
        public IEnumerable<IMatcher> MatchingRules { get; protected set; }

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

            //Populate matching rules collection
            var matchingRules = new List<IMatcher>();
            matchingRules.Add(new DefaultRequestBodyMatcher());


            //If no matching rules can be found, we can then add the default one or apply a default one all the time.
            //Something like the law of specicifity on the path?
            MatchingRules = matchingRules;


            //Return the example value with rules stripped

            return body;
        }

        public string PathWithQuery()
        {
            if (String.IsNullOrEmpty(Path) && !String.IsNullOrEmpty(Query))
            {
                throw new InvalidOperationException("Query has been supplied, however Path has not. Please specify as Path.");
            }

            return !String.IsNullOrEmpty(Query) ?
                    String.Format("{0}?{1}", Path, Query) :
                    Path;
        }
    }
}