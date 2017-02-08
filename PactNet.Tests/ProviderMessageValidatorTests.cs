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
using Newtonsoft.Json.Linq;
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

            MessagingPactFile pactFile = new MessagingPactFile();

            Assert.Throws<ArgumentException>(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_ThrowsExecptionWithNoProviderInPactFile()
        {
            ProviderMessageValidator sut = GetSystemUnderTest(new MockMessanger());

            MessagingPactFile pactFile = new MessagingPactFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };

            Assert.Throws<ArgumentException>(() => sut.Validate(pactFile));
        }

        [Fact]
        public void Validate_OkWithNoMessagesInPactFile()
        {
            ProviderMessageValidator sut = GetSystemUnderTest(new MockMessanger());

            MessagingPactFile pactFile = new MessagingPactFile();
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

            MessagingPactFile pactFile = new MessagingPactFile();
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

            MessagingPactFile pactFile = new MessagingPactFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            var body = new PactDslJsonRoot()
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

            MessagingPactFile pactFile = new MessagingPactFile();
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

            MessagingPactFile pactFile = new MessagingPactFile();
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

            MessagingPactFile pactFile = new MessagingPactFile();
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

            MessagingPactFile pactFile = new MessagingPactFile();
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

        [Fact]
        public void Validator_Reports_ComparisonResults_Appropriately()
        {
            var dsl = new PactDslJsonRoot()
                .Object("a")
                    .StringType("a1", "test1")
                    .StringType("a2", "test2")
                    .IntegerMatcher("a3", 3)
                    .StringMatcher("a4", "([a-z]).*", "test4")
                    .DecimalMatcher("a5", decimal.Parse("5.03234"))
                    .EqualityMatcher("a6", "test6")
                    .EqualityMatcher("a7", 7)
                    .EqualityMatcher("a8", decimal.Parse("8.123"))
                    .DateFormat("a9", "MM/dd/yyyy", DateTime.UtcNow)
                    .TimestampFormat("a10", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'", DateTime.Now)
                    .MinArrayLike("b", 1)
                        .Item(new PactDslJsonBody()
                            .StringType("c1", "test6")
                            .StringMatcher("c3", "([a-z]).*", "test7")
                            .Object("d")
                                .Int32Type("d1", 8)
                                .StringType("d2", "test9")
                            .CloseObject())
                        .Item(new PactDslJsonBody()
                            .StringType("c1", "test6")
                            .StringMatcher("c3", "([a-z]).*", "test7")
                            .Object("d")
                                .Int32Type("d1", 8)
                                .StringType("d2", "test9")
                            .CloseObject())
                    .CloseArray()
                    .Object("e")
                        .StringType("e1", "test10")
                        .Int32Type("e2", 11)
                    .CloseObject()
                .CloseObject();

            var badmsg1 = "{\r\n    \"a\": {\r\n        \"a1\": \"test1\",\r\n        \"a10\": \"2017-01-26T09:11:53.SSSZ\",\r\n        \"a2\": \"test2\",\r\n        \"a3\": \"3\",\r\n        \"a5\": \"5.03234\",\r\n        \"a6\": \"test6\",\r\n        \"a7\": 8,\r\n        \"a8\": 8.123,\r\n        \"a9\": \"01/26/2017\",\r\n        \"b\": [\r\n            {\r\n                \"d\": {\r\n                    \"d1\": 8,\r\n                    \"d2\": \"test9\"\r\n                }\r\n            },\r\n            {\r\n                \"c1\": \"test6\",\r\n                \"c3\": \"test7\",\r\n                \"d\": {\r\n                    \"d1\": 8,\r\n                    \"d2\": \"test9\"\r\n                }\r\n            }\r\n        ],\r\n        \"e\": {\r\n            \"e1\": \"test10\",\r\n            \"e2\": 11\r\n        }\r\n    }\r\n}";
            var badmsg2 = "{\r\n    \"a\": {\r\n        \"a1\": \"test1\",\r\n        \"a10\": \"01-26-2017T09:11:53.SSSZ\",\r\n        \"a2\": \"test2\",\r\n        \"a3\": 3,\r\n        \"a4\": \"test4\",\r\n        \"a5\": 5.03234,\r\n        \"a6\": \"test6\",\r\n        \"a7\": 7,\r\n        \"a8\": 8.123,\r\n        \"a9\": \"01-27-2017\",\r\n        \"b\": [\r\n            {\r\n                \"c1\": \"test6\",\r\n                \"c3\": \"test7\",\r\n                \"d\": {\r\n                    \"d1\": 8,\r\n                    \"d2\": \"test9\"\r\n                }\r\n            },\r\n            {\r\n                \"c1\": \"test6\",\r\n                \"c3\": \"test7\",\r\n                \"d\": {\r\n                    \"d1\": 8,\r\n                    \"d2\": \"test9\"\r\n                }\r\n            }\r\n        ],\r\n        \"e\": {\r\n            \"e1\": \"test10\",\r\n            \"e2\": 11\r\n        }\r\n    }\r\n}";
            var messager = new MockMessanger();

            messager.Publish("my.test.topic", "my provider state", JObject.Parse(badmsg1));
            messager.Publish("my.second.test.topic", "my provider state 2", JObject.Parse(badmsg2));

            ProviderMessageValidator sut = GetSystemUnderTest(messager);

            MessagingPactFile pactFile = new MessagingPactFile();
            pactFile.Consumer = new PactNet.Models.Pacticipant() { Name = "consumer" };
            pactFile.Provider = new PactNet.Models.Pacticipant() { Name = "provider" };

            pactFile.AddMessage(new Message()
                .Given("I can create a report!")
                .ExpectsToRecieve("my.test.topic")
                .WithBody(dsl));

            pactFile.AddMessage(new Message()
                .Given("I can create a second report!")
                .ExpectsToRecieve("my.second.test.topic")
                .WithBody(dsl));

            Assert.Throws<PactFailureException>(() => sut.Validate(pactFile));
        }
    }
}
