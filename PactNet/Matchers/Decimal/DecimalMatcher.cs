using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Decimal
{
    public class DecimalMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return DecimalMatchDefinition.Name; }
        }

        public DecimalMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            throw new NotImplementedException();
        }
    }
}