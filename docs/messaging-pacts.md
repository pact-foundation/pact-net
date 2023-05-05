Messaging Pacts
===============

Messaging pacts work similarly to synchronous request/response pacts in that there is still a consumer
and a producer. The consumer defines the messages it expects to receive (including metadata and the
message contents) and the producer is verified to ensure that the messages it produces meet those
expectations.

It's important to ensure that any given consumer or provider name doesn't have both request/response and
messaging pacts. If you have a HTTP API which also sends and/or receives messages, make sure the two different
types use two different names, for example "Stock Broker API" and "Stock Broker Messaging".

Sample
------

See the [sample](../samples/Messaging/) for additional detail.

Consumer Tests
--------------

Consumer tests are very similar to request/response pacts. Your consumer specifies which messages it wishes
to receive and PactNet generates a pact file which contains all of the specified interactions.

In code, this is:

```csharp
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
                new XUnitOutput(output)
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
```

After all of your consumer tests have passed a message pact file is written to disk. This file will be used
during the provider verification tests.

Provider Tests
--------------

Provider tests look very similar to request/response pacts, but with one big difference; you must register a
handler for each interaction which generates a sample message.

The key difference for mesaging pacts is that the transport used is not via HTTP. It would be unreasonable
for PactNet to attempt to implement all the different transports that these messages could use - such as
Kafka, RabbitMQ, ZeroMQ, etc - and so internally the messages are simulated during the provider verification
stage. This is done transparently and so you don't need to worry about how this is achieved.

In code, this is:

```csharp
public class StockEventGeneratorTests : IDisposable
{
    private readonly PactVerifier verifier;

    public StockEventGeneratorTests()
    {
        this.verifier = new PactVerifier("Stock Event Producer");
    }

    public void Dispose()
    {
        // make sure you dispose the verifier to stop the internal messaging server
        GC.SuppressFinalize(this);
        this.verifier.Dispose();
    }

    [Fact]
    public void EnsureEventApiHonoursPactWithConsumer()
    {
        string pactPath = Path.Combine("..",
                                       "..",
                                       "..",
                                       "..",
                                       "Consumer.Tests",
                                       "pacts",
                                       "Stock Event Consumer-Stock Event Producer.json");

        var defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        this.verifier
            .MessagingProvider(scenarios =>
            {
                // register the responses to each interaction
                // the descriptions must match those in the pact file(s)
                scenarios.Add("a single event", () => new StockEvent
                         {
                             Name = "AAPL",
                             Price = 1.23m
                         })
                         .Add("some stock ticker events", builder =>
                         {
                             builder.WithMetadata(new
                                    {
                                        ContentType = "application/json",
                                        Key = "value"
                                    })
                                    .WithContent(new[]
                                    {
                                        new StockEvent { Name = "AAPL", Price = 1.23m },
                                        new StockEvent { Name = "TSLA", Price = 4.56m }
                                    });
                         });
            }, defaultSettings)
            .WithFileSource(new FileInfo(pactPath))
            .Verify();
    }
}
```
