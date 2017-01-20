using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Equality
{
    public class EqualityMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return EqualityMatchDefinition.Name; }
        }

        public EqualityMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            throw new NotImplementedException();
        }
    }
}