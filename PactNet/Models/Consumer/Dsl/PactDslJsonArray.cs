using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;

namespace PactNet.Models.Consumer.Dsl
{
    public class PactDslJsonArray : DslPart<List<DslPart>>
    {
        protected int Size;

        public PactDslJsonArray()
            : base()
        {
            Body = new List<DslPart>();
        }

        public PactDslJsonArray(DslPart parent, string rootName, int size)
            : base(parent, rootName)
        {
            Size = size;
            Body = new List<DslPart>();
        }

        public override string Path
        {
            get
            {
                var path = string.Empty;
                if (_parent != null)
                    path = string.Format("{0}.{1}[*]", _parent.Path, _rootName);
                else
                    path = "$" + _rootName;

                return path;
            }
        }

        public override Dictionary<string, IMatcher> Matchers
        {
            get
            {
                var matchers = new Dictionary<string, IMatcher>();
                foreach (var part in this.Body)
                {
                    if (part.Matchers == null)
                        continue;

                    foreach (var match in _matchers)
                        matchers[this.Path.Replace("[*]", string.Empty)] = match;

                    foreach (var match in part.Matchers)
                        matchers[match.Key] = match.Value;
                }

                if (matchers.Count > 0)
                    return matchers;

                return null;
            }
            set { _matchers.AddRange(value.Values); }
        }

        public override Dictionary<string, object> Content
        {
            get
            {
                var content = new Dictionary<string, object>();

                content[this.Name] = this.Value;

                if (content.Count > 0)
                    return content;

                return null;
            }
            set { Console.WriteLine(value); }
        }

        public override object Value
        {
            get
            {
                //We have a primitive like array
                if (this.Body.TrueForAll(x => x.IsPrimitive))
                {
                    var values = new List<object>();

                    foreach (var parts in this.Body)
                        values.Add(parts.Value);

                    return values;
                }

                var items = new List<Dictionary<string, object>>();

                foreach (var parts in this.Body)
                    items.Add(parts.Content);

                return items;
            }
        }

        public override bool IsPrimitive
        {
            get { return false; }
        }

        public override MatcherResult Validate(JToken message)
        {
            var result = new MatcherResult();

            foreach (var matcher in _matchers)
            {
                var path = this.Path.Replace("[*]", string.Empty);
                result.Add(matcher.Match(path, JToken.FromObject(this.Value), message.SelectToken(path)));
            }

            //TODO: How do we validate missing elements of an array item?

            foreach (var item in this.Body)
                result.Add(item.Validate(message));

            return result;
        }

        public PactDslJsonBody CloseArray()
        {
            return (PactDslJsonBody) _parent;
        }

        public PactDslJsonArray Item(PactDslJsonBody body)
        {
            if (this.Body.Count > 0 && !(this.Body[0] is PactDslJsonBody))
                throw new InvalidOperationException("Items added to the array must be the same type");

            body.Parent = this;
            this.Body.Add(body);
            return this;
        }

        public PactDslJsonArray Item<T>(T value) where T : IConvertible
        {
            if (this.Body.Count > 0 && !(this.Body[0] is PactDslValue<T>))
                throw new InvalidOperationException("Invalid type added to array. Array items must be the same type");

            //TODO: Support for different matchers for an array item
            var item = new PactDslValue<T>(this, string.Empty, value).TypeMatcher();
            this.Body.Add(item);
            return this;
        }

        public PactDslJsonArray MinMatcher(int minItems)
        {
            MatchMinValue(minItems);
            return this;
        }

        public PactDslJsonArray MaxMatcher(int maxItems)
        {
            MatchMaxValue(maxItems);
            return this;
        }
    }
}
