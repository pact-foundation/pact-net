using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PactNet.Matchers;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    public class PactDslJsonBody : DslPart<Dictionary<string, DslPart>>
    {
        [JsonProperty("matchingRules",  NullValueHandling = NullValueHandling.Ignore)]
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
                    content[parts.Name] = parts.Content;
                }

                if (content.Count > 0)
                    return content;

                return null;
            }
        }

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

        public PactDslJsonBody Object(string name)
        {
            Body[name] = new PactDslJsonBody(this, name);
            return (PactDslJsonBody)Body[name];
        }

        public PactDslJsonBody CloseObject()
        {
            if (_parent != null)
                return (PactDslJsonBody)_parent;

            return this;
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

        public PactDslJsonBody StringMatcher(string name, string regex, string example)
        {
            Body[name] = new PactDslValue<string>(this, name, example).StringMatcher(regex);
            return this;
        }

    }
}
