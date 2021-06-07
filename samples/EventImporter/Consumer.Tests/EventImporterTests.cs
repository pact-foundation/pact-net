using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Consumer.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using PactNet.Native;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class EventImporterTests
    {
        private const string Token = "SomeValidAuthToken";

        private readonly IPactBuilderV3 pact;
        private readonly IPactMessageBuilderV3 pactMessage;

        private PactConfig config = new PactConfig
        {
            LogDir = "../../../logs/",
            PactDir = "../../../pacts/",
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        public EventImporterTests(ITestOutputHelper output)
        {
            config.Outputters = new[]
            {
                new XUnitOutput(output)
            };
            IPactV3 pact = Pact.V3("Event API Consumer V3", "Event API V3", config);
            this.pact = pact.UsingNativeBackend();

            IPactV3 pactMessageV3 = Pact.V3("Event API Consumer V3 Message", "Event API V3 Message", config);
            pactMessage = pactMessageV3.NewMessage();
        }

        [Fact]
        public async Task GetAllEvents_FromQueue_Should_CreatePact_WithMessages()
        {
            var expected = new List<Event> 
            {
                new Event(){
                    EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548"),
                    EventType = "SearchView"
                }
            };

            var worker = new EventsWorker(new FakeEventProcessor());

            pactMessage
                .ExpectsToReceive("receiving events from the queue")
                .Given("A list of events is pushed to the queue")
                .WithMetadata("key", "valueKey")
                .WithContent(Match.Type(expected))
                .Verify(() => worker.ProcessMessages(expected));

            pactMessage.Build();

            //var client = new EventsApiClient(context.MockServerUri, Token);

            //IEnumerable<Event> events = await client.GetAllEvents();

            //events.Should().BeEquivalentTo(expected);
        }
    }
}
