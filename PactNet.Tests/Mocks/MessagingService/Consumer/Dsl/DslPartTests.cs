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
        public void Path_Serializes_()
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

            var test = JsonConvert.SerializeObject(dsl);
        }
    }
}
