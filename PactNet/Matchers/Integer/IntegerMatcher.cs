using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Integer
{
    public class IntegerMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return IntegerMatchDefinition.Name; }
        }

        public IntegerMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            throw new NotImplementedException();
        }
    }
}