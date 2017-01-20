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
        private Dictionary<string, object> _matchers;
        private Dictionary<string, object> _content;

        public override DslPart Create(Type objectType)
        {
            return new PactDslJsonBody();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var body = (PactDslJsonBody)base.ReadJson(reader, objectType, existingValue, serializer);

            _matchers = body.Matchers;
            _content = body.Content;

            var root = new PactDslJsonBody();



            foreach (var item in _content)
                root.Body.Add(item.Key, this.BuildPactDsl((JToken)item.Value, root, item.Key));

            return root;
        }

        private DslPart BuildPactDsl(JToken token, DslPart parent, string rootName)
        {
            JValue value;
            switch (token.Type)
            {
                case JTokenType.Object:
                    var obj = token as JObject;
                    if (parent is PactDslJsonArray)
                        ((PactDslJsonArray)parent).Body.Add(this.BuildPactDslJsonBody(parent, obj, rootName));

                    parent = this.BuildPactDslJsonBody(parent, obj, rootName);
                    break;
                case JTokenType.Array:
                    var array = token as JArray;
                    parent = this.BuildPactDslArray(parent, array, rootName);
                    break;
                case JTokenType.Property:
                    var property = token as JProperty;
                    ((PactDslJsonBody)parent).Body[property.Name] = this.BuildPactDsl(property.First, parent, property.Name);
                    break;
                case JTokenType.String:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<string>());
                case JTokenType.Integer:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<int>());
                case JTokenType.Boolean:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<bool>());
            }

            return parent;
        }

        private PactDslJsonBody BuildPactDslJsonBody(DslPart parent, JObject obj, string rootName)
        {
            var pactDslBody = new PactDslJsonBody(parent, rootName);
            foreach (var child in obj.Children())
                this.BuildPactDsl(child, pactDslBody, rootName);

            return pactDslBody;
        }

        private PactDslJsonArray BuildPactDslArray(DslPart parent, JArray array, string rootName)
        {
            var pactDslArray = new PactDslJsonArray(parent, rootName, 1);
            foreach (var item in array.Children())
                this.BuildPactDsl(item, pactDslArray, string.Empty);

            return pactDslArray;
        }

        private PactDslValue<T> BuildPactDslValue<T>(DslPart parent, string name, T example) where T : IConvertible
        {
            var value = new PactDslValue<T>(parent, name, example);

            if (_matchers.ContainsKey(value.Path))
            {
                var matchers = (JToken)_matchers[value.Path];

                foreach (JProperty matcher in matchers.Children())
                {
                    switch (matcher.Name)
                    {
                        case "match":
                            value.TypeMatcher();
                            break;
                        case "regex":
                            value.StringMatcher(matcher.First.Value<string>());
                            break;
                        case "date":
                            value.DateFormatMatcher(matcher.First.Value<string>());
                            break;
                    }
                }
            }


            return value;
        }


    }
}
