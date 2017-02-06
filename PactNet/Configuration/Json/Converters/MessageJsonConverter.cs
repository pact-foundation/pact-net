using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PactNet.Models.Messaging;

namespace PactNet.Configuration.Json.Converters
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

            message.Body = new DslPartJsonConverter(message.Contents, message.MatchingRules).Build();

            return message;
        }
    }
}
