using Newtonsoft.Json;

namespace PactNet.Matchers.Max
{
    public class MaxMatchDefinition : MatchDefinition
    {
        public const string Name = "max";

        [JsonProperty("max")]
        public int MaxValue { get; protected set; }

        public MaxMatchDefinition(object example, int maxValue) :
            base(Name, example)
        {
            MaxValue = maxValue;
        }
    }
}