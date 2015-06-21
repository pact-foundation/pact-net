using Newtonsoft.Json;

namespace PactNet.Matchers
{
    public abstract class MatchDefinition
    {
        [JsonProperty("$pactMatcherType")]
        public string Type { get; protected set; }

        [JsonProperty("example")]
        public object Example { get; protected set; }

        protected MatchDefinition(string type, object example)
        {
            Type = type;
            Example = example;
        }
    }
}