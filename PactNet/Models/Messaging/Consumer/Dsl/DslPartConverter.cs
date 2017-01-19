using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;

namespace PactNet.Models.Messaging.Consumer.Dsl
{
    public class DslPartConverter : CustomCreationConverter<DslPart>
    {
        public override DslPart Create(Type objectType)
        {
            return new PactDslJsonBody();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken root = JToken.Load(reader);

            

            foreach (var child in root.Children())
            {
                var token = child;
                switch (token.Path)
                {
                    case "matchingRules":
                        var matchers = this.BuildMatchers(token);
                        break;
                    case "contents":
                        var contents = this.BuildContents(token);
                        break;
                }
            }

            throw new NotImplementedException();
        }

        private Dictionary<string, List<IMatcher>> BuildMatchers(JToken token)
        {
            var matchers = new Dictionary<string, List<IMatcher>>();

            foreach (var child in token.Children())
            {
                token = child;

                while (token.HasValues)
                {
                    token = token.Next;
                    //TODO: build flat matchers structure and then figure out how to use JsonPath to map it back to the DslPart structure
                }
            }

            return matchers;
        }

        private Dictionary<string, object> BuildContents(JToken token)
        {
            var contents = new Dictionary<string, object>();

            while (token.HasValues)
            {
                token = token.Next;
                //TODO: re-build DslPart structure
            }

            return contents;
        }
    }
}
