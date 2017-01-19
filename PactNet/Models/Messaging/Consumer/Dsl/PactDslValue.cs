using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Matchers;

namespace PactNet.Models.Messaging.Consumer.Dsl
{
    public class PactDslValue<T> : DslPart<T> where T : IConvertible
    {
        public PactDslValue()
            :base()
        {
        }

        public PactDslValue(DslPart parent, string rootName, T value)
            :base(parent, rootName)
        {
            Body = value;
        }

        public override Dictionary<string, object> Content
        {
            get { return new Dictionary<string, object> {{_rootName, this.Body}}; }
            set { Console.WriteLine(value); }
        }

        public override Dictionary<string, object> Matchers
        {
            get
            {
                var matchers = new Dictionary<string, object>();
                foreach (var matcher in _matchers)
                    matchers[this.Path] = matcher.Value;

                //TODO: This serializes incorrectly. only uses the last IMatcher in the collection. Update to match the V3 spec:
                /*
                 * {
                      "matchingRules": {
                        "path": {
                            "matchers": [
                              {"match": "A"}
                            ]
                        }
                      }
                    }
                 */

                return matchers;
            }
            set { Console.WriteLine(value);}
        }

        public override bool IsPrimitive { get { return true; } }

        public override object Value { get { return this.Body; } }

        public PactDslValue<T> DateFormatMatcher(string dateFormat)
        {
            return (PactDslValue<T>) this.MatchDateFormat(dateFormat);
        }

        public PactDslValue<T> TypeMatcher()
        {
            return (PactDslValue<T>) this.MatchType();
        }

        public PactDslValue<T> StringMatcher(string regex)
        {
            return (PactDslValue<T>)this.MatchRegex(regex);
        }
    }
}
