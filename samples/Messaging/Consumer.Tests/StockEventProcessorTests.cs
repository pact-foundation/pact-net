using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class StockEventProcessorTests
    {
        private readonly IMessagePactBuilderV3 messagePact;

        public StockEventProcessorTests(ITestOutputHelper output)
        {
            IPactV3 v3 = Pact.V3("Stock Event Consumer", "Stock Event Producer", new PactConfig
            {
                PactDir = "../../../pacts/",
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                },
                Outputters = new[]
                {
                    new XunitOutput(output)
                }
            });

            this.messagePact = v3.WithMessageInteractions();
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
