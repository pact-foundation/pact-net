using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class StockEventProcessorTests
    {
        private readonly IMessagePactBuilderV3 messagePact;

        public StockEventProcessorTests(ITestOutputHelper output)
        {
            IMessagePactV3 v3 = MessagePact.V3("Stock Event Consumer", "Stock Event Producer", new PactConfig
            {
                PactDir = "../../../pacts/",
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                },
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });

            this.messagePact = v3.UsingNativeBackend();
        }

        [Fact]
        public void ReceiveSomeStockEvents()
        {
            this.messagePact
                .ExpectsToReceive("some stock ticker events")
                .Given("A list of events is pushed to the queue")
                .WithMetadata("key", "valueKey")
                .WithJsonContent(Match.MinType(new
                {
                    Name = Match.Type("AAPL"),
                    Price = Match.Decimal(1.23m),
                    Timestamp = Match.Type(14.February(2022).At(13, 14, 15, 678))
                }, 1))
                .Verify<ICollection<StockEvent>>(events =>
                {
                    events.Should().BeEquivalentTo(new[]
                    {
                        new StockEvent
                        {
                            Name = "AAPL",
                            Price = 1.23m,
                            Timestamp = 14.February(2022).At(13, 14, 15, 678)
                        }
                    });
                });
        }
    }
}
