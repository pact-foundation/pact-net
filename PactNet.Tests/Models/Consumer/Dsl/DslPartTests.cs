using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Configuration.Json.Converters;
using PactNet.Matchers;
using PactNet.Models.Consumer.Dsl;
using Xunit;

namespace PactNet.Tests.Models.Consumer.Dsl
{
    public class DslPartTests
    {
        [Fact]
        public void Path_Builds_Recursively()
        {
            var dsl = new PactDslJsonRoot()
                .Object("a")
                    .Object("b")
                        .Object("c");

            Assert.Equal("$.body.a.b.c", dsl.Path);
        }

        [Fact]
        public void PactDslJsonBody_Serializes_Content_And_Matchers()
        {
            var dsl = new PactDslJsonRoot()
                .Object("a")
                    .StringType("a1", "test1")
                    .StringType("a2", "test2")
                    .IntegerMatcher("a3", 3)
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

            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a2\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a3\": {\r\n      \"match\": \"integer\"\r\n    },\r\n    \"$.body.a.a4\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b.b1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b.c.c1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b.c.c2\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    }\r\n  },\r\n  \"contents\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4a\",\r\n      \"b\": {\r\n        \"b1\": \"test5\",\r\n        \"c\": {\r\n          \"c1\": 5,\r\n          \"c2\": \"test6\"\r\n        }\r\n      }\r\n    }\r\n  }\r\n}";
            var actual = JsonConvert.SerializeObject(dsl, Formatting.Indented);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PactDslJsonBody_Parses_Dynamic_Content_And_Matchers()
        {
            var body = new
            {
                a = new
                {
                    a1 = Match.Type("test1"),
                    a2 = Match.Type("test2"),
                    a3 = Match.Integer(3),
                    //a4 = Match.Type("test4"), //TODO: how to we do multiple matchers?
                    a4 = Match.Regex("test4a", "([a-z]).*"),
                    b = new
                    {
                        b1 = Match.Type("test5"),
                        c = new
                        {
                            c1 = Match.Type(5),
                            c2 = Match.Regex("test6", "([a-z]).*")
                        }
                    }
                }
            };

            var dsl = PactDslJsonBody.Parse(body);
            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a2\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a3\": {\r\n      \"match\": \"integer\"\r\n    },\r\n    \"$.body.a.a4\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b.b1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b.c.c1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b.c.c2\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    }\r\n  },\r\n  \"contents\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4a\",\r\n      \"b\": {\r\n        \"b1\": \"test5\",\r\n        \"c\": {\r\n          \"c1\": 5,\r\n          \"c2\": \"test6\"\r\n        }\r\n      }\r\n    }\r\n  }\r\n}"; var actual = JsonConvert.SerializeObject(dsl, Formatting.Indented);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PactDslJsonArray_Serializes_Content_And_Matchers()
        {
            var dsl = new PactDslJsonRoot()
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
                    .Object("e")
                        .StringType("e1", "test10")
                        .Int32Type("e2", 11)
                    .CloseObject()
                .CloseObject();

            var expected = "{\r\n  \"matchingRules\": {\r\n    \"$.body.a.a1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a2\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a3\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.a4\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b\": {\r\n      \"min\": 1\r\n    },\r\n    \"$.body.a.b[*].c1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b[*].c3\": {\r\n      \"regex\": \"([a-z]).*\"\r\n    },\r\n    \"$.body.a.b[*].d.d1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.b[*].d.d2\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.e.e1\": {\r\n      \"match\": \"type\"\r\n    },\r\n    \"$.body.a.e.e2\": {\r\n      \"match\": \"type\"\r\n    }\r\n  },\r\n  \"contents\": {\r\n    \"a\": {\r\n      \"a1\": \"test1\",\r\n      \"a2\": \"test2\",\r\n      \"a3\": 3,\r\n      \"a4\": \"test4\",\r\n      \"b\": [\r\n        {\r\n          \"c1\": \"test6\",\r\n          \"c3\": \"test7\",\r\n          \"d\": {\r\n            \"d1\": 8,\r\n            \"d2\": \"test9\"\r\n          }\r\n        },\r\n        {\r\n          \"c1\": \"test6\",\r\n          \"c3\": \"test7\",\r\n          \"d\": {\r\n            \"d1\": 8,\r\n            \"d2\": \"test9\"\r\n          }\r\n        }\r\n      ],\r\n      \"e\": {\r\n        \"e1\": \"test10\",\r\n        \"e2\": 11\r\n      }\r\n    }\r\n  }\r\n}";
            var actual = JsonConvert.SerializeObject(dsl, Formatting.Indented);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Pact_Deserializes_Into_PactDslJsonBody()
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

            var expected = JsonConvert.SerializeObject(dsl);
            var actual = JsonConvert.DeserializeObject<PactDslJsonBody>(expected, new DslPartJsonConverter());

            Assert.Equal(expected, JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public void DslPart_Validates_Matchers()
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
                    .DateFormat("a9","MM/dd/yyyy",DateTime.UtcNow)
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
                    .MinArrayLike("x", 1)
                        .Item("item1")
                        .Item("item2")
                    .CloseArray()
                .CloseObject()
                .StringType("z", "ztesttoremove");
              

            var message = new JObject();
            var content = JObject.FromObject(dsl.Content);

            var removed = content.Remove("z");
            message.Add("body", content);


            var results = dsl.Validate(message);

            Assert.Equal(35, results.MatcherChecks.Count);
            Assert.Equal(1, results.MatcherChecks.Count(m => m.GetType() == typeof(FailedMatcherCheck)));
        }
    }
}
