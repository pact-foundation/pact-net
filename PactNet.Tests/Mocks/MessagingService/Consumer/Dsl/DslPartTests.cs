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
                .StringValue("a1", "test1")
                .StringValue("a2", "test2")
                .Int32Value("a3", 3)
                    .Object("b")
                    .StringValue("b1", "test4")
                        .Object("c")
                        .Int32Value("c1", 5)
                        .CloseObject()
                    .CloseObject()
                .CloseObject();

            var test = JsonConvert.SerializeObject(dsl);
        }
    }
}
