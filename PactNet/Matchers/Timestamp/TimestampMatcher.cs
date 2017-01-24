using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Timestamp
{
    public class TimestampMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return TimestampDefinition.Name; }
        }

        [JsonProperty("timestamp")]
        public string Format { get; protected set; }
        
        public TimestampMatcher(string format)
        {
            Format = format;
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JValue;

            DateTime dateTime;
            var matches = act != null &&
                          DateTime.TryParseExact(act.Value.ToString(), this.Format, CultureInfo.InvariantCulture,
                              DateTimeStyles.None, out dateTime);

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotMatchTimestamp));
        }
    }
}