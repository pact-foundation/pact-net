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

            var expected = "{\"matchingRules\":{\"$.body.a.a1\":[{\"match\":\"type\"}],\"$.body.a.a2\":[{\"match\":\"type\"}],\"$.body.a.a3\":[{\"match\":\"type\"}],\"$.body.a.a4\":[{\"regex\":\"([a-z]).*\"}],\"$.body.a.b.b1\":[{\"match\":\"type\"}],\"$.body.a.b.c.c1\":[{\"match\":\"type\"}],\"$.body.a.b.c.c2\":[{\"regex\":\"([a-z]).*\"}]},\"content\":{\"a\":{\"a1\":{\"a1\":\"test1\"},\"a2\":{\"a2\":\"test2\"},\"a3\":{\"a3\":3},\"a4\":{\"a4\":\"test4\"},\"b\":{\"b1\":{\"b1\":\"test5\"},\"c\":{\"c1\":{\"c1\":5},\"c2\":{\"c2\":\"test6\"}}}}}}";
           Assert.Equal(expected, JsonConvert.SerializeObject(dsl));
        }
    }
}
