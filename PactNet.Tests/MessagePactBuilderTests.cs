using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using PactNet;
using PactNet.Models.Messaging.Consumer.Dsl;

namespace PactNet.Tests
{
    public class MessagePactBuilderTests
    {
        [Fact]
        public void Can_Add_A_Message()
        {
            var body = new PactDslJsonRoot()
                .StringType("foo", "bar");

            Dictionary<string, object> metaData = new Dictionary<string, object>();
            metaData.Add("contentType", "application/json" );

            PactMessageBuilder builder = new PactMessageBuilder();
            Message m = new Message()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                Body = body
            };


            builder.WithContent(m)
                .WithMetaData(metaData);
        }

        [Fact]
        public void Creates_Pact_Properly()
        {
            var body = new PactDslJsonRoot()
                .StringType("foo", "bar");

            Dictionary<string, object> metaData = new Dictionary<string, object>();
            metaData.Add("contentType", "application/json");

            PactMessageBuilder builder = new PactMessageBuilder();
            builder.ServiceConsumer("Consumer");
            builder.HasPactWith("Provider");

            Message m = new Message()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                MetaData = metaData,
                Body = body
            };

            builder.WithContent(m);

            const string expectedPact = "{\"provider\":{\"name\":\"Provider\"},\"consumer\":{\"name\":\"Consumer\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe \'scenario\'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"matchingRules\":{\"$.body.foo\":{\"match\":\"type\"}},\"metaData\":{\"contentType\":\"application/json\"}}],\"metadata\":{\"pact-specification\":\"3.0.0\",\"pact-net\":\"0.0.0.1\"}}";
            string actual = builder.GetPactAsJSON();
            Assert.Equal<string>(expectedPact, actual);
        }

        [Fact]
        public void Consumes_Pact_Properly()
        {
            var body = new PactDslJsonRoot()
                .StringType("foo", "bar");

            Message m = new Message()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                Body = body
            };

            string expected = JsonConvert.SerializeObject(m);
            Message actual = JsonConvert.DeserializeObject<Message>(expected);

            Assert.Equal(expected, JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public void Saves_Pact_To_Disk()
        {
            var body = new PactDslJsonRoot()
                .StringType("foo", "bar");

            Dictionary<string, object> metaData = new Dictionary<string, object>();
            metaData.Add("contentType", "application/json");

            var config = new PactConfig();

            IPactMessagingBuilder builder = new PactMessageBuilder(config);

            builder.ServiceConsumer("Consumer")
                .HasPactWith("Provider");

            builder
                .WithContent(new Message()
                .Given("or maybe 'scenario'? not sure about this")
                .ExpectsToRecieve("my.random.topic")
                .WithMetaData(metaData)
                .WithBody(body))
            .WithContent(new Message()
                .Given("Pact Message can support multiple messages")
                .ExpectsToRecieve("my.second.random.topic")
                .WithBody(body))
            .Build();
        }
    }
}
