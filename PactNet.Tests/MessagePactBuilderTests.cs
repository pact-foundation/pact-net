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
            var body = new PactDslJsonBody()
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
            var body = new PactDslJsonBody()
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
                Body = body
            };

            builder.WithContent(m)
             .WithMetaData(metaData);

            const string expectedPact = "{\"provider\":{\"name\":\"Provider\"},\"consumer\":{\"name\":\"Consumer\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe \'scenario\'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"matchingRules\":{\"$.body.foo\":{\"match\":\"type\"}}}],\"metaData\":{\"contentType\":\"application/json\"}}";
            string actual = builder.GetPactAsJSON();
            Assert.Equal<string>(expectedPact, actual);
        }

        [Fact]
        public void Consumes_Pact_Properly()
        {
            var body = new PactDslJsonBody()
                .StringType("foo", "bar");

            Message m = new Message()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                Body = body
            };

            string expected = JsonConvert.SerializeObject(m);
            Message actual = JsonConvert.DeserializeObject<Message>(expected, new MessageJsonConverter());

            Assert.Equal(expected, JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public void Saves_Pact_To_Disk()
        {
            var body = new PactDslJsonBody()
                .StringType("foo", "bar");

            Dictionary<string, object> metaData = new Dictionary<string, object>();
            metaData.Add("contentType", "application/json");

            PactMessageBuilder builder = new PactMessageBuilder();
            builder.ServiceConsumer("Consumer");
            builder.HasPactWith("Provider");
           
            Message m = new Message()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "my.random.topic",
                Body = body
            };

            builder.WithContent(m)
              .WithMetaData(metaData);

            builder.Build();

            //var pactConfig = new PactConfig();

            //string expectedFileName = "consumer-provider.json";

            //var generatedPact = File.ReadAllText(pactConfig + expectedFileName);

            //Assert.Equal<string>(builder.GetPactAsJSON(), generatedPact);
        }
    }
}
