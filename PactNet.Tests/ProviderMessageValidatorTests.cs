using Newtonsoft.Json;
using PactNet.Mocks;
using PactNet.Mocks.MockMessager;
using PactNet.Models.Messaging;
using PactNet.Models.Messaging.Consumer.Dsl;
using PactNet.Reporters;
using PactNet.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PactNet.Tests
{
    public class ProviderMessageValidatorTests
    {

        class TestMessage
        {
            [JsonProperty("myStringProperty")]
            public string MyStringProperty { get; set; }

            [JsonProperty("myIntProperty")]
            public int MyIntProperty { get; set; }
        }

        class SecondaryTestMessage
        {
            [JsonProperty("myGuidProperty")]
            public Guid MyGuidProperty { get; set; }

            [JsonProperty("iHaveAStringPropertyAlso")]
            public string IHaveAStringPropertyAlso { get; set; }
        }

        private ProviderMessageValidator GetSystemUnderTest(IMockMessager messager)
        {
            PactVerifierConfig config = new PactVerifierConfig();

            ProviderMessageValidator validator = new ProviderMessageValidator(new Reporter(config), config, messager);
                

            return validator;
        }

        [Fact]
        public void Validate_ThrowsExceptionWithNullPactFile()
        {
            ProviderMessageValidator sut = GetSystemUnderTest(new MockMessanger());

            Assert.Throws<ArgumentException>(() => sut.Validate(null));
        }

        [Fact]
        public void Validate_ThrowsExecptionWithNoConsumerInPactFile()
        {
            ProviderMessageValidator sut = GetSystemUnderTest(new MockMessanger());

            PactMessageFile pactFile = new PactMessageFile();

            Assert.Throws<ArgumentException>(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_ThrowsExecptionWithNoProviderInPactFile()
        {
            ProviderMessageValidator sut = GetSystemUnderTest(new MockMessanger());

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };

            Assert.Throws<ArgumentException>(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_OkWithNoMessagesInPactFile()
        {
            ProviderMessageValidator sut = GetSystemUnderTest(new MockMessanger());

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };

            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            Assert.DoesNotThrow(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_ThrowsExecptionWhenNoMessageMatchesDescription()
        {
            var messager = new MockMessanger();
            messager.Publish("my.test.topic", "my provider state", "HI there");

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            var body = new PactDslJsonBody()
             .Object("partyInvite")
                 .StringType("eventType", "My Event")
             .CloseObject();

            Dictionary<string, object> metaData = new Dictionary<string, object>();


            Message myPactMessage = new Message()
            {
                Description = "Not.My.Topic",
                ProviderState = "some radom state",
                Body = body,
                MetaData = metaData
            };

            pactFile.AddMessage(myPactMessage);

            Assert.Throws<PactFailureException>(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_ThrowsExecptionWhenPactDoesNotMatch()
        {
            var messager = new MockMessanger();
            messager.Publish("my.test.topic", "my provider state", "HI there");

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            var body = new PactDslJsonBody()
             .Object("partyInvite")
                 .Int32Type("eventType", 1)
             .CloseObject();

            Dictionary<string, object> metaData = new Dictionary<string, object>();

            Message myPactMessage = new Message()
            {
                Description = "my.test.topic",
                ProviderState = "some radom state",
                Body = body,
                MetaData = metaData
            };

            pactFile.AddMessage(myPactMessage);

            Assert.Throws<PactFailureException>(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_OkWhenPactMatches()
        {
            var messager = new MockMessanger();
            TestMessage tm = new TestMessage()
            {
                MyIntProperty = 1,
                MyStringProperty = "HI there"
            };

            messager.Publish("my.test.topic", "my provider state", tm);

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            var body = new PactDslJsonRoot()
                .IntegerMatcher("myIntProperty", 11)
                .StringType("myStringProperty", "example");
            
            Dictionary<string, object> metaData = new Dictionary<string, object>();

            Message myPactMessage = new Message()
            {
                Description = "my.test.topic",
                ProviderState = "some radom state",
                Body = body,
                MetaData = metaData
            };

            pactFile.AddMessage(myPactMessage);

            Assert.DoesNotThrow(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_FiltersMessagesBasedOnDescription()
        {
            var messager = new MockMessanger();
            TestMessage tm = new TestMessage()
            {
                MyIntProperty = 1,
                MyStringProperty = "HI there"
            };

            SecondaryTestMessage tm2 = new SecondaryTestMessage()
            {
                MyGuidProperty = Guid.NewGuid(),
                IHaveAStringPropertyAlso = "Another String property"
            };

            messager.Publish("my.test.topic", "my provider state", tm);
            messager.Publish("my.second.topic", "my second provider state", tm2);

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            var body = new PactDslJsonRoot()
                 .IntegerMatcher("myIntProperty", 11)
                 .StringType("myStringProperty", "example");
            

            Dictionary<string, object> metaData = new Dictionary<string, object>();

            Message myPactMessage = new Message()
            {
                Description = "my.test.topic",
                ProviderState = "some radom state",
                Body = body,
                MetaData = metaData
            };

            pactFile.AddMessage(myPactMessage);

            Assert.DoesNotThrow(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_FiltersMessagesBasedOnProviderState()
        {
            var messager = new MockMessanger();
            TestMessage tm = new TestMessage()
            {
                MyIntProperty = 1,
                MyStringProperty = "HI there"
            };

            SecondaryTestMessage tm2 = new SecondaryTestMessage()
            {
                MyGuidProperty = Guid.NewGuid(),
                IHaveAStringPropertyAlso = "Another String property"
            };

            messager.Publish("my.test.topic", "my provider state", tm);
            messager.Publish("my.second.topic", "my second provider state", tm2);

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            const string guidRegEx = "[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}";

            var body = new PactDslJsonRoot()
                 .StringMatcher("myGuidProperty", guidRegEx, Guid.NewGuid().ToString())
                 .StringType("iHaveAStringPropertyAlso", "yippie");
             

            Dictionary<string, object> metaData = new Dictionary<string, object>();

            Message myPactMessage = new Message()
            {
                Description = "my",
                ProviderState = "my second provider state",
                Body = body,
                MetaData = metaData
            };

            pactFile.AddMessage(myPactMessage);

            Assert.DoesNotThrow(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_OkForMultipleMessages()
        {
            var messager = new MockMessanger();
            TestMessage tm = new TestMessage()
            {
                MyIntProperty = 1,
                MyStringProperty = "HI there"
            };

            SecondaryTestMessage tm2 = new SecondaryTestMessage()
            {
                MyGuidProperty = Guid.NewGuid(),
                IHaveAStringPropertyAlso = "Another String property"
            };

            messager.Publish("my.test.topic", "my provider state", tm);
            messager.Publish("my.second.topic", "my second provider state", tm2);

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            PactMessageFile pactFile = new PactMessageFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            string guidRegEx = "";

            var body = new PactDslJsonRoot()
                 .StringMatcher("myGuidProperty", guidRegEx, Guid.NewGuid().ToString())
                 .StringType("iHaveAStringPropertyAlso", "yippie");

            Dictionary<string, object> metaData = new Dictionary<string, object>();

            Message myPactMessage = new Message()
            {
                Description = "my",
                ProviderState = "my second provider state",
                Body = body,
                MetaData = metaData
            };

            var body2 = new PactDslJsonRoot()
                .IntegerMatcher("myIntProperty", 11)
                 .StringType("myStringProperty", "example");

            Message myPactMessage2 = new Message()
            {
                Description = "my.test.topic",
                ProviderState = "my provider state",
                Body = body2,
                MetaData = metaData
            };

            pactFile.AddMessage(myPactMessage);
            pactFile.AddMessage(myPactMessage2);

            Assert.DoesNotThrow(() => sut.Validate(pactFile));
        }
    }
}
