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
        }

        public override Dictionary<string, List<IMatcher>> Matchers
        {
            get { return new Dictionary<string, List<IMatcher>> {{this.Path, _matchers.Values.ToList()}}; }
        }

        public override bool IsPrimitive { get { return true; } }

        public override object Value { get { return this.Body; } }

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
