using System;
using Newtonsoft.Json;
using PactNet.Models.Messaging.Consumer.Dsl;
using Xunit;

namespace PactNet.Tests.Models.Consumer.Dsl
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
        public void PactDslJsonBody_Serializes_Content_And_Matchers()
        {
            var dsl = new PactDslJsonBody()
                .Object("a")
                    .StringType("a1", "test1")
                    .StringType("a2", "test2")
                    .Int32Type("a3", 3)
                    .StringType("a4", "test4a")
                    .StringMatcher("a4", "([a-z]).*", "test4") //Only this one will be included because we're still using V2 spec for matchers
                    .Object("b")
                        .StringType("b1", "test5")
                            .Object("c")
                                .Int32Type("c1", 5)
                                .StringMatcher("c2", "([a-z]).*", "test6")
                            .CloseObject()
                    .CloseObject()
                .CloseObject();

            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a2\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a3\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a4\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b.b1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b.c.c1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b.c.c2\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    }\r\n  },\r\n  \"contents\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4a\",\r\n      \"b\": {\r\n        \"b1\": \"test5\",\r\n        \"c\": {\r\n          \"c1\": 5,\r\n          \"c2\": \"test6\"\r\n        }\r\n      }\r\n    }\r\n  }\r\n}";
            var actual = JsonConvert.SerializeObject(dsl, Formatting.Indented);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PactDslJsonArray_Serializes_Content_And_Matchers()
        {
            var dsl = new PactDslJsonBody()
                .Object("a")
                    .StringType("a1", "test1")
                    .StringType("a2", "test2")
                    .Int32Type("a3", 3)
                    .StringMatcher("a4", "([a-z]).*", "test4")
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
                .CloseObject();

            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a2\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a3\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a4\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b[*].c1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b[*].c3\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b[*].d.d1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b[*].d.d2\": {\r\n      \"match\": \"type\"\r\n    }\r\n  },\r\n  \"contents\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4\",\r\n      \"b\": [\r\n        {\r\n          \"c1\": \"test6\",\r\n          \"c3\": \"test7\",\r\n          \"d\": {\r\n            \"d1\": 8,\r\n            \"d2\": \"test9\"\r\n          }\r\n        },\r\n        {\r\n          \"c1\": \"test6\",\r\n          \"c3\": \"test7\",\r\n          \"d\": {\r\n            \"d1\": 8,\r\n            \"d2\": \"test9\"\r\n          }\r\n        }\r\n      ]\r\n    }\r\n  }\r\n}";
            var actual = JsonConvert.SerializeObject(dsl, Formatting.Indented);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Pact_Deserializes_Into_PactDslJsonBody()
        {
            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.a2\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.a3\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.a4\": [\r\n      {\r\n        \"regex\": \"([a-z]).*\"\r\n      }\r\n    ],\r\n    \"$.body.a.b[*].c1\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.b[*].c3\": [\r\n      {\r\n        \"regex\": \"([a-z]).*\"\r\n      }\r\n    ],\r\n    \"$.body.a.b[*].d.d1\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ],\r\n    \"$.body.a.b[*].d.d2\": [\r\n      {\r\n        \"match\": \"type\"\r\n      }\r\n    ]\r\n  },\r\n  \"contents\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4\",\r\n      \"b\": [\r\n        {\r\n          \"c1\": \"test6\",\r\n          \"c3\": \"test7\",\r\n          \"d\": {\r\n            \"d1\": 8,\r\n            \"d2\": \"test9\"\r\n          }\r\n        },\r\n        {\r\n          \"c1\": \"test6\",\r\n          \"c3\": \"test7\",\r\n          \"d\": {\r\n            \"d1\": 8,\r\n            \"d2\": \"test9\"\r\n          }\r\n        }\r\n      ]\r\n    }\r\n  }\r\n}";

            Console.Write(JsonConvert.DeserializeObject<PactDslJsonBody>(expected, new DslPartConverter()));
        }
    }
}
