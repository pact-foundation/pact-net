using System;
using System.Collections.Generic;
using PactNet.Matchers;

namespace PactNet.Models.Messaging.Consumer.Dsl
{
    public class PactDslJsonArray : DslPart<List<PactDslJsonBody>>
    {
        protected int Size;

        public PactDslJsonArray()
            : base()
        {
            Body = new List<PactDslJsonBody>();
        }

        public PactDslJsonArray(DslPart parent, string rootName, int size)
            : base(parent, rootName)
        {
            Size = size;
            Body = new List<PactDslJsonBody>();
        }

        public override string Path
        {
            get
            {
                var path = string.Empty;
                if (_parent != null)
                    path = string.Format("{0}.{1}[*]", _parent.Path, _rootName);
                else
                    path = "$.body" + _rootName;

                return path;
            }
        }

        public override Dictionary<string, object> Matchers
        {
            get
            {
                var matchers = new Dictionary<string, object>();
                foreach (var part in this.Body)
                {
                    if (part.Matchers == null)
                        continue;

                    foreach (var match in part.Matchers)
                        matchers[match.Key] = match.Value;
                }

                if (matchers.Count > 0)
                    return matchers;

                return null;
            }
            set { Console.WriteLine(value); }
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

        public PactDslJsonBody CloseArray()
        {
            return (PactDslJsonBody) _parent;
        }

        public PactDslJsonArray Item(PactDslJsonBody body)
        {
            body.Parent = this;
            this.Body.Add(body);
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
