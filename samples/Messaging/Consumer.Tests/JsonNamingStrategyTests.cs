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
    public class JsonNamingStrategyTests
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IMessagePactBuilderV3 messagePact;

        public JsonNamingStrategyTests(ITestOutputHelper output)
        {
            IMessagePactV3 v3 = MessagePact.V3("Extended Stock Event Consumer", "Extended Stock Event Producer", new PactConfig
            {
                PactDir = "../../../pacts/",
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
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
                .ExpectsToReceive("some extended stock ticker events")
                .Given("A list of extended events is pushed to the queue")
                .WithMetadata("key", "valueKey")
                .WithJsonContent(Match.MinType(new
                {
                    Name = Match.Type("AAPL"),
                    Price = Match.Decimal(1.23m),
                    Timestamp = Match.Type(14.February(2022).At(13, 14, 15, 678)),
                    NamingStrategyTest = Match.Type("Value for NamingStrategyTest property")
                }, 1))
                .Verify<ICollection<StockEventExt>>(events =>
                {
                    events.Should().BeEquivalentTo(new[]
                    {
                        new StockEventExt
                        {
                            Name = "AAPL",
                            Price = 1.23m,
                            Timestamp = 14.February(2022).At(13, 14, 15, 678),
                            NamingStrategyTest = "Value for NamingStrategyTest property"
                        }
                    });
                });
        }
    }
}
