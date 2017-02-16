using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PactNet.Matchers.Date;
using PactNet.Matchers.Decimal;
using PactNet.Matchers.Equality;
using PactNet.Matchers.Integer;
using PactNet.Matchers.Max;
using PactNet.Matchers.Min;
using PactNet.Matchers.Regex;
using PactNet.Matchers.Timestamp;
using PactNet.Matchers.Type;

namespace PactNet.Matchers
{
    public class MatcherFactory : Dictionary<string, Func<JContainer, IMatcher>>
    {
        public MatcherFactory()
        {
            this[TypeMatchDefinition.Name] = props => new TypeMatcher();
            this[RegexMatchDefinition.Name] = props => new RegexMatcher(props["regex"].Value<string>());
            this[EqualityMatchDefinition.Name] = props => new EqualityMatcher();
            this[IntegerMatchDefinition.Name] = props => new IntegerMatcher();
            this[DecimalMatchDefinition.Name] = props => new DecimalMatcher();
            this[MinMatchDefinition.Name] = props => new MinMatcher(props["min"].Value<int>());
            this[MaxMatchDefinition.Name] = props => new MaxMatcher(props["ma"].Value<int>());
            this[DateFormatMatchDefinition.Name] = props => new DateFormatMatcher(props["date"].Value<string>());
            this[TimestampDefinition.Name] = props => new TimestampMatcher(props["timestamp"].Value<string>());
        }
    }
}
