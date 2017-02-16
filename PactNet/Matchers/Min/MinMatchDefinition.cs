using Newtonsoft.Json;

namespace PactNet.Matchers.Min
{
    public class MinMatchDefinition : MatchDefinition
    {
        public const string Name = "min";

        [JsonProperty("min")]
        public int MinValue { get; protected set; }

        public MinMatchDefinition(object example, int minValue) :
            base(Name, example)
        {
            MinValue = minValue;
        }
    }
}