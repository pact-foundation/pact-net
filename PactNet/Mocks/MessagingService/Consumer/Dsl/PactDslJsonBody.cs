using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PactNet.Matchers;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    public class PactDslJsonBody : DslPart<Dictionary<string, DslPart>>
    {

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
        public override Dictionary<string, List<IMatcher>> Matchers
        {
            get
            {
                var matchers = new Dictionary<string, List<IMatcher>>();
                foreach (var parts in this.Body.Values)
                {
                    foreach (var match in parts.Matchers)
                        matchers[match.Key] = match.Value;
                }

                if (matchers.Count > 0)
                    return matchers;

                return null;
            }
        }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public override Dictionary<string, object> Content
        {
            get
            {
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
        }

        public override bool IsPrimitive { get { return false; } }

        public override object Value { get { return this.Body; } }

        public DslPart Parent
        {
            get { return _parent; }
            set { _parent = value; }
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
            Body[name] = new PactDslJsonArray(this, name, size);
            return ((PactDslJsonArray) Body[name]);
        }

        public PactDslJsonBody StringType(string name, string example)
        {
            Body[name] = new PactDslValue<string>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody Int32Type(string name, int example)
        {
            Body[name] = new PactDslValue<int>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody DecimalType(string name, int example)
        {
            Body[name] = new PactDslValue<decimal>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody StringMatcher(string name, string regex, string example)
        {
            Body[name] = new PactDslValue<string>(this, name, example).StringMatcher(regex);
            return this;
        }

        public PactDslJsonBody GuidType(string name, Guid example)
        {
            const string guidRegEx = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";
            Body[name] = new PactDslValue<string>(this, name, example.ToString()).StringMatcher(guidRegEx);
            return this;
        }

        public PactDslJsonBody DateTimeType(string name, DateTime example)
        {
            Body[name] = new PactDslValue<DateTime>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody DoubleType(string name, double example)
        {
            Body[name] = new PactDslValue<double>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody DecimalType(string name, decimal example)
        {
            Body[name] = new PactDslValue<decimal>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody FloatType(string name, float example)
        {
            Body[name] = new PactDslValue<float>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody Int64Type(string name, Int64 example)
        {
            Body[name] = new PactDslValue<Int64>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody ShortType(string name, short example)
        {
            Body[name] = new PactDslValue<short>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody LongType(string name, long example)
        {
            Body[name] = new PactDslValue<long>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody UShortType(string name, ushort example)
        {
            Body[name] = new PactDslValue<ushort>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody UIntType(string name, uint example)
        {
            Body[name] = new PactDslValue<uint>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody UInt64Type(string name, UInt64 example)
        {
            Body[name] = new PactDslValue<UInt64>(this, name, example).TypeMatcher();
            return this;
        }

        public PactDslJsonBody ULongType(string name, ulong example)
        {
            Body[name] = new PactDslValue<ulong>(this, name, example).TypeMatcher();
            return this;
        }
    }
}
