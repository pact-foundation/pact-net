using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;

namespace PactNet.Models.Messaging.Consumer.Dsl
{
    public class PactDslJsonBody : DslPart<Dictionary<string, DslPart>>
    {
        private Dictionary<string, object> _matchersJObject;
        private Dictionary<string, object> _contentJObject;

        public PactDslJsonBody()
            :base()
        {
            Body = new Dictionary<string, DslPart>();
        }

        public PactDslJsonBody(DslPart parent, string rootName)
            :base(parent, rootName)
        {
            Body = new Dictionary<string, DslPart>();
        }

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public override Dictionary<string, object> Matchers
        {
            get
            {
                if (_matchersJObject != null)
                    return _matchersJObject;

                var matchers = new Dictionary<string, object>();
                foreach (var parts in this.Body.Values)
                {
                    if (parts.Matchers == null)
                        continue;
                    
                    foreach (var match in parts.Matchers)
                        matchers[match.Key] = match.Value;
                }

                if (matchers.Count > 0)
                    return matchers;

                return null;
            }
            set { _matchersJObject = value; }
        }

        [JsonProperty("contents", NullValueHandling = NullValueHandling.Ignore)]
        public override Dictionary<string, object> Content
        {
            get
            {
                if (_contentJObject != null)
                    return _contentJObject;

                var content = new Dictionary<string, object>();
                foreach (var parts in this.Body.Values)
                {
                    if (parts.IsPrimitive)
                        content[parts.Name] = parts.Value;
                    else if (parts is PactDslJsonArray)
                        content[parts.Name] = parts.Value;
                    else
                        content[parts.Name] = parts.Content;
                }

                if (content.Count > 0)
                    return content;

                return null;
            }
            set { _contentJObject = value; }
        }

        public override bool IsPrimitive { get { return false; } }

        public override object Value { get { return this.Body; } }

        public override MatcherResult Validate(JToken message)
        {
            var result = new MatcherResult();

            foreach(var matcher in _matchers.Values)
                result.Add(matcher.Match(this.Path, JToken.FromObject(this.Value), message.SelectToken(this.Path)));

            foreach(var item in this.Body.Values)
                result.Add(item.Validate(message));

            return result;
        }

        private PactDslValue<T> GetItem<T>(string name, T example) where T : IConvertible
        {
            if (!Body.ContainsKey(name))
                Body[name] = new PactDslValue<T>(this, name, example);

            return (PactDslValue<T>)Body[name];
        }

        public PactDslJsonBody Object(string name)
        {
            Body[name] = new PactDslJsonBody(this, name);
            return (PactDslJsonBody)Body[name];
        }

        public PactDslJsonBody CloseObject()
        {
            if (_parent == null)
                return this;

            return (PactDslJsonBody) _parent;
        }

        public PactDslJsonArray MinArrayLike(string name, int size)
        {
            Body[name] = new PactDslJsonArray(this, name, size).MinMatcher(size);
            return ((PactDslJsonArray) Body[name]);
        }

        public PactDslJsonArray MaxArrayLike(string name, int size)
        {
            Body[name] = new PactDslJsonArray(this, name, size).MaxMatcher(size);
            return ((PactDslJsonArray)Body[name]);
        }

        public PactDslJsonBody StringMatcher(string name, string regex, string example)
        {
            this.GetItem(name, example).StringMatcher(regex);
            return this;
        }

        public PactDslJsonBody GuidMatcher(string name, Guid example)
        {
            const string guidRegEx = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
            this.GetItem(name, example.ToString()).StringMatcher(guidRegEx);
            return this;
        }

        public PactDslJsonBody DateFormat(string name, string dateFormat, DateTime example)
        {
            this.GetItem(name, example).DateFormatMatcher(dateFormat);
            return this;
        }

        public PactDslJsonBody TimestampFormat(string name, string format, DateTime example)
        {
            this.GetItem(name, example).TimestampMatcher(format);
            return this;
        }

        #region TypeMatchers
        public PactDslJsonBody StringType(string name, string example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody Int32Type(string name, int example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody DecimalType(string name, int example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody BooleanType(string name, bool example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody DoubleType(string name, double example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody DecimalType(string name, decimal example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody FloatType(string name, float example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody Int64Type(string name, Int64 example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody ShortType(string name, short example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody LongType(string name, long example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody UShortType(string name, ushort example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody UIntType(string name, uint example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody UInt64Type(string name, UInt64 example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody ULongType(string name, ulong example)
        {
            this.GetItem(name, example).TypeMatcher();
            return this;
        }
        #endregion
    }
}
