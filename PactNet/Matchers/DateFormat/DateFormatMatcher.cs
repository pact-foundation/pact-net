using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.DateFormat
{
    public class DateFormatMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return DateFormatMatchDefinition.Name; }
        }

        [JsonProperty("date")]
        public string DateFormat { get; protected set; }
        
        public DateFormatMatcher(string dateFormat)
        {
            DateFormat = dateFormat;
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            throw new NotImplementedException();
        }
    }
}