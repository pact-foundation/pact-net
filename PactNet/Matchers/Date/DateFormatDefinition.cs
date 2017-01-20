using Newtonsoft.Json;

namespace PactNet.Matchers.Date
{
    public class DateFormatMatchDefinition : MatchDefinition
    {
        public const string Name = "date";

        [JsonProperty("date")]
        public string DateFormat { get; protected set; }

        public DateFormatMatchDefinition(object example, string dateFormat) :
            base(Name, example)
        {
            DateFormat = dateFormat;
        }
    }
}