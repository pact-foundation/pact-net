using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Matchers;
using PactNet.Matchers.Date;
using PactNet.Matchers.Decimal;
using PactNet.Matchers.Equality;
using PactNet.Matchers.Integer;
using PactNet.Matchers.Max;
using PactNet.Matchers.Min;
using PactNet.Matchers.Regex;
using PactNet.Matchers.Timestamp;
using PactNet.Matchers.Type;

namespace PactNet.Models.Messaging.Consumer.Dsl
{
    public abstract class DslPart
    {
        protected DslPart _parent;
        protected string _rootName;
        protected Dictionary<string, IMatcher> _matchers;

        protected DslPart()
            : this(null, string.Empty)
        {
        }

        protected DslPart(DslPart parent, string rootName)
        {
            _parent = parent;
            _rootName = rootName;

            _matchers = new Dictionary<string, IMatcher>();
        }

        [JsonIgnore]
        public DslPart Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        [JsonIgnore]
        public string Name { get { return _rootName; } }

        [JsonIgnore]
        public virtual string Path
        {
            get
            {
                var path = string.Empty;
                if (_parent != null)
                    if (!string.IsNullOrEmpty(_rootName))
                        path = string.Format("{0}.{1}", _parent.Path, _rootName);
                    else
                        path = _parent.Path;
                else
                    path = "$.body" + _rootName;

                return path;
            }
        }

        public abstract Dictionary<string, object> Matchers { get; set; }
        public abstract Dictionary<string, object> Content { get; set; }

        [JsonIgnore]
        public abstract object Value { get; }

        [JsonIgnore]
        public abstract bool IsPrimitive { get; }

        protected DslPart MatchType()
        {
            _matchers["type"] = new TypeMatcher();
            return this;
        }

        protected DslPart MatchEquality()
        {
            _matchers["equality"] = new EqualityMatcher();
            return this;
        }

        protected DslPart MatchInteger()
        {
            _matchers["integer"] = new IntegerMatcher();
            return this;
        }

        protected DslPart MatchDecimal()
        {
            _matchers["decimal"] = new DecimalMatcher();
            return this;
        }

        protected DslPart MatchRegex(string regex)
        {
            _matchers["regex"] = new RegexMatcher(regex);
            return this;
        }

        protected DslPart MatchDateFormat(string format)
        {
            _matchers["date"] = new DateFormatMatcher(format);
            return this;
        }

        protected DslPart MatchTimestamp(string format)
        {
            _matchers["timestamp"] = new TimestampMatcher(format);
            return this;
        }

        protected DslPart MatchMinValue(int minValue)
        {
            _matchers["min"] = new MinMatcher(minValue);
            return this;
        }

        protected DslPart MatchMaxValue(int maxValue)
        {
            _matchers["max"] = new MaxMatcher(maxValue);
            return this;
        }
    }

    public abstract class DslPart<T> : DslPart
    {
        [JsonIgnore]
        public T Body;

        protected DslPart()
        {
        }

        protected DslPart(DslPart parent, string rootName)
            :base(parent, rootName)
        {
        }

    }
}
