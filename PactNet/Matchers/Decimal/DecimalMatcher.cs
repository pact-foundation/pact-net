using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Decimal
{
    public class DecimalMatcher : IMatcher
    {
        public string Type
        {
            get { return DecimalMatchDefinition.Name; }
        }

        public DecimalMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JValue;
            double decValue;

            //Check if it's null
            if (act == null)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotExist, "Decimal", "(null)"));

            //Check if it's numeric
            if (act.Type != JTokenType.Float && act.Type != JTokenType.Integer)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotDecimal, "Decimal", string.Format("{0} ({1})", act.Value, act.Type)));

            var matches = act.Type == JTokenType.Float && double.TryParse(act.Value.ToString(), out decValue);

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path, "Decimal", act.Value)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotDecimal, "Decimal", act.Value));
        }
    }
}