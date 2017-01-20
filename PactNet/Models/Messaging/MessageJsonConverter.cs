using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PactNet.Models.Messaging.Consumer.Dsl;

namespace PactNet.Models.Messaging
{
    public class MessageJsonConverter : CustomCreationConverter<Message>
    {
        public override Message Create(Type objectType)
        {
            return new Message();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var message = (Message)base.ReadJson(reader, objectType, existingValue, serializer);

            var dslPartConverter = new DslPartJsonConverter(message.Contents, message.MatchingRules);

            var root = new PactDslJsonBody();
            foreach (var item in message.Contents)
                if (item.Value is JToken)
                    root.Body.Add(item.Key, dslPartConverter.BuildPactDsl((JToken)item.Value, root, item.Key));
                else
                    root.Body.Add(item.Key, dslPartConverter.BuildPactDslValue(root, item.Key, item.Value.ToString()));

            message.Body = root;

            return message;
        }
    }
}
