using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using PactNet.Verifier.Messaging;
using Xunit;

namespace PactNet.Tests.Verifier.Messaging
{
    public class MessageScenariosTests
    {
        private readonly IMessageScenarios scenarios;

        public MessageScenariosTests()
        {
            this.scenarios = new MessageScenarios();
        }

        [Fact]
        public void Add_SimpleScenario_AddsScenarioWithJsonMetadata()
        {
            Func<dynamic> factory = () => new { Foo = 42 };

            this.scenarios.Add("description", factory);

            this.scenarios.Scenarios.Should().BeEquivalentTo(new Dictionary<string, Scenario>
            {
                ["description"] = new Scenario("description", factory, new { ContentType = "application/json" }, null)
            });
        }

        [Fact]
        public void Add_SyncBuilder_AddsScenario()
        {
            object metadata = new { Key = "value" };
            Func<dynamic> factory = () => new { Foo = 42 };
            JsonSerializerOptions settings = new JsonSerializerOptions();

            this.scenarios.Add("description", builder => builder.WithMetadata(metadata).WithContent(factory, settings));

            this.scenarios.Scenarios.Should().BeEquivalentTo(new Dictionary<string, Scenario>
            {
                ["description"] = new Scenario("description", factory, metadata, settings)
            });
        }

        [Fact]
        public void Add_AsyncBuilder_AddsScenario()
        {
            object metadata = new { Key = "value" };
            Func<Task<dynamic>> factory = () => Task.FromResult<dynamic>(new { Foo = 42 });
            JsonSerializerOptions settings = new JsonSerializerOptions();

            this.scenarios.Add("description", builder => builder.WithMetadata(metadata).WithAsyncContent(factory, settings));

            this.scenarios.Scenarios.Should().BeEquivalentTo(new Dictionary<string, Scenario>
            {
                ["description"] = new Scenario("description", factory, metadata, settings)
            });
        }
    }
}
