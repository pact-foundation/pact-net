using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PactNet.Mocks.MessagingService;
using PactNet.Mocks.MessagingService.Consumer.Dsl;
using PactNet.Models.Messaging;
using Xunit;

namespace PactNet.Tests.Mocks.MessagingService.Consumer.Dsl
{
    public class DslPartTests
    {
        [Fact]
        public void Path_Builds_Recursively()
        {
            var dsl = new PactDslJsonBody()
                .Object("a")
                    .Object("b")
                        .Object("c");

            Assert.Equal("$.body.a.b.c", dsl.Path);
        }

        [Fact]
        public void Path_Serializes_Content_And_Matchers()
        {
            var dsl = new PactDslJsonBody()
                .Object("a")
                    .StringType("a1", "test1")
                    .StringType("a2", "test2")
                    .Int32Type("a3", 3)
                    .StringMatcher("a4", "([a-z]).*", "test4")
                    .Object("b")
                        .StringType("b1", "test5")
                            .Object("c")
                                .Int32Type("c1", 5)
                                .StringMatcher("c2", "([a-z]).*", "test6")
                            .CloseObject()
                    .CloseObject()
                .CloseObject();

            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.a2\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.a3\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.a4\": [\r\n      {\r\n        \"regex\": \"([a-z]).*\"\r\n      }\r\n    ],\r\n    \"$.body.a.b.b1\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.b.c.c1\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.b.c.c2\": [\r\n      {\r\n        \"regex\": \"([a-z]).*\"\r\n      }\r\n    ]\r\n  },\r\n  \"content\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4\"\r\n    },\r\n    \"b\": {\r\n      \"b1\": \"test5\",\r\n      \"c\": {\r\n        \"c1\": 5,\r\n        \"c2\": \"test6\"\r\n      }\r\n    }\r\n  }\r\n}";
            Assert.Equal(expected, JsonConvert.SerializeObject(dsl, Formatting.Indented));
        }
    }
}
