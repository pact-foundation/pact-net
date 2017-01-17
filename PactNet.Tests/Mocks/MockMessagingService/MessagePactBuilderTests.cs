using PactNet.Mocks.MessagingService;
using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PactNet.Tests.Mocks.MockMessagingService
{
    public class MessagePactBuilderTests
    {
        [Fact]
        public void Can_Add_A_Message()
        {
            MessagePactBuilder<string> builder = new MessagePactBuilder<string>();
            Message<string> m = new Message<string>()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                MetaData = new MetaData() {  ContentType = "application/json" }
            };
            m.Contents.Add("foo", "bar");

            builder.AddMessage(m);
        }

        [Fact]
        public void Can_Get_A_Message()
        {
            MessagePactBuilder<string> builder = new MessagePactBuilder<string>();
            Message<string> m = new Message<string>()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                MetaData = new MetaData() { ContentType = "application/json" }
            };
            m.Contents.Add("foo", "bar");
            builder.AddMessage(m);

            var returnedMessage = builder.GetMessage();

            Assert.Equal<string>(m.ProviderState, returnedMessage.ProviderState);
            Assert.Equal<string>(m.Description, returnedMessage.Description);
            Assert.Equal<string>(m.MetaData.ContentType, returnedMessage.MetaData.ContentType);
            Assert.True(m.Contents.ContainsKey("foo"));
            Assert.Equal<string>("bar", m.Contents["foo"]);

        }

        [Fact]
        public void Returns_Null_When_No_Messages()
        {
            MessagePactBuilder<string> builder = new MessagePactBuilder<string>();

            var message = builder.GetMessage();

            Assert.Null(message);
        }

        [Fact]
        public void Creates_Pact_Properly()
        {
            MessagePactBuilder<string> builder = new MessagePactBuilder<string>();
            builder.ServiceConsumer("Consumer");
            builder.HasPactWith("Provider");

            Message<string> m = new Message<string>()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                MetaData = new MetaData() { ContentType = "application/json" }
            };

            m.Contents.Add("foo", "bar");

            builder.AddMessage(m);
            
            const string expectedPact = "{\"provider\":{\"name\":\"Provider\"},\"consumer\":{\"name\":\"Consumer\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe 'scenario'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"metaData\":{\"contentType\":\"application/json\"}}]}";
            string actual = builder.GetPactAsJSON();
            Assert.Equal<string>(expectedPact, actual);
        }

        [Fact]
        public void Saves_Pact_To_Disk()
        {
            MessagePactBuilder<string> builder = new MessagePactBuilder<string>();
            builder.ServiceConsumer("Consumer");
            builder.HasPactWith("Provider");
            builder.ExceptsToRecieve("my.random.topic");

            Message<string> m = new Message<string>()
            {
                ProviderState = "or maybe 'scenario'? not sure about this",
                Description = "Published credit data",
                MetaData = new MetaData() { ContentType = "application/json" }
            };

            m.Contents.Add("foo", "bar");

            builder.AddMessage(m);

            builder.Build();

            //var pactConfig = new PactConfig();

            //string expectedFileName = "consumer-provider-my-random-topic.json";

            //var generatedPact = File.ReadAllText(pactConfig + expectedFileName);

            //Assert.Equal<string>(builder.GetPactAsJSON(), generatedPact);
        }
    }
}
