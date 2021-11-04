using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Consumer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class EventImporterTests
    {
        private readonly IMessagePactBuilderV3 messagePact;

        private readonly PactConfig config = new PactConfig
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
            this.config.Outputters = new[]
            {
                new XUnitOutput(output)
            };

            IMessagePactV3 v3 = MessagePact.V3("Event API Consumer V3 Message", "Event API V3 Message", this.config);
            this.messagePact = v3.UsingNativeBackend();
        }

        [Fact]
        public async Task GetAllEvents_FromQueue_Should_CreatePact_WithMessages()
        {
            var worker = new EventsWorker(new FakeEventProcessor());

            this.messagePact
                .ExpectsToReceive("receiving events from the queue")
                .Given("A list of events is pushed to the queue")
                .WithMetadata("key", "valueKey")
                .WithJsonContent(
                    Match.MinType(new[]
                    {
                        new
                        {
                            EventId = Match.Regex(Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5").ToString(),
                                "(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)"),
                            Timestamp = Match.Type(DateTime.Parse("2014-06-30T01:37:41.0660548")),
                            EventType = Match.Type("SearchView")
                        }
                    }, 1));

            await this.messagePact.VerifyAsync<List<Event>>(events => worker.ProcessMessages(events));
        }
    }
}
