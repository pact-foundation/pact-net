using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;
using PactNet.Models.Consumer.Dsl;

namespace PactNet.Configuration.Json.Converters
{
    public class DslPartJsonConverter : CustomCreationConverter<DslPart>
    {
        private Dictionary<string, IMatcher> _matchers;
        private Dictionary<string, object> _content;

        public DslPartJsonConverter()
        {
            
        }

        public DslPartJsonConverter(Dictionary<string, object> content, Dictionary<string, IMatcher> matchers)
        {
            _content = content;
            _matchers = matchers;
        }

        public override DslPart Create(Type objectType)
        {
            return new PactDslJsonBody();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var body = (PactDslJsonBody)base.ReadJson(reader, objectType, existingValue, serializer);

            _matchers = body.Matchers;
            _content = body.Content;

            return this.Build();
        }

        public PactDslJsonRoot Build()
        {
            var root = new PactDslJsonRoot();

            foreach (var item in _content)
            {
                var token = item.Value as JToken ?? JToken.FromObject(item.Value);
                //TODO: throw an exception if null maybe?
                //TODO: Handle if root is a JArray
                //TODO: Handle if root is JValue
                root.Body.Add(item.Key, this.BuildPactDsl(token, root, item.Key));
            }

            return root;
        }

        public DslPart BuildPactDsl(JToken token, DslPart parent, string rootName)
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
                case JTokenType.Guid:
                case JTokenType.Date:
                case JTokenType.TimeSpan:
                case JTokenType.Uri:
                case JTokenType.Undefined:
                case JTokenType.String:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<string>());
                case JTokenType.Integer:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<int>());
                case JTokenType.Boolean:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<bool>());
                case JTokenType.Float:
                    value = token as JValue;
                    return this.BuildPactDslValue(parent, rootName, value.Value<decimal>());
            }

            return parent;
        }

        public PactDslJsonBody BuildPactDslJsonBody(DslPart parent, JObject obj, string rootName)
        {
            var pactDslBody = new PactDslJsonBody(parent, rootName);
            foreach (var child in obj.Children())
                this.BuildPactDsl(child, pactDslBody, rootName);

            return pactDslBody;
        }

        public PactDslJsonArray BuildPactDslArray(DslPart parent, JArray array, string rootName)
        {
            var pactDslArray = new PactDslJsonArray(parent, rootName, array.Count);
            foreach (var item in array.Children())
                this.BuildPactDsl(item, pactDslArray, string.Empty);

            var path = pactDslArray.Path.Replace("[*]", string.Empty);

            if (_matchers.ContainsKey(path))
            {
                pactDslArray.Matchers = new Dictionary<string, IMatcher> { { String.Empty, _matchers[path] } };
            }

            return pactDslArray;
        }

        public PactDslValue<T> BuildPactDslValue<T>(DslPart parent, string name, T example) where T : IConvertible
        {
            var value = new PactDslValue<T>(parent, name, example);

            //TODO: need to handle a collection of matchers here
            if (_matchers.ContainsKey(value.Path))
            {
                value.Matchers = new Dictionary<string, IMatcher> { {String.Empty, _matchers[value.Path]} };
            }

            return value;
        }


    }
}
