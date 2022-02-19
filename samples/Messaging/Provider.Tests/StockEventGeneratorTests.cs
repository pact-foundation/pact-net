using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class StockEventGeneratorTests : IDisposable
    {
        private readonly PactVerifier verifier;

        public StockEventGeneratorTests(ITestOutputHelper output)
        {
            this.verifier = new PactVerifier(new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XunitOutput(output)
                },
                LogLevel = PactLogLevel.Debug
            });
        }

        public void Dispose()
        {
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
                .MessagingProvider("Stock Event Producer", defaultSettings)
                .WithProviderMessages(scenarios =>
                 {
                     scenarios.Add("a single event",
                                   () => new StockEvent
                                   {
                                       Name = "AAPL",
                                       Price = 1.23m,
                                       Timestamp = new DateTimeOffset(2022, 2, 14, 13, 14, 15, TimeSpan.Zero)
                                   })
                              .Add("some stock ticker events", SetupStockEvents);
                 })
                .WithFileSource(new FileInfo(pactPath))
                .Verify();
        }

        private static async Task SetupStockEvents(IMessageScenarioBuilder builder)
        {
            //-----------------------------------------------------
            // A complex example of scenario setting.
            //
            // - here we chose to generate the object from the business component
            //   by mocking the last call before sending information to the queue
            //-----------------------------------------------------
            await builder.WithMetadata(new
                          {
                              ContentType = "application/json",
                              Key = "valueKey"
                          })
                         .WithContentAsync(async () =>
                          {
                              //The component that will send the message to the queue (Kafka, Rabbit, etc)
                              var mockSender = new Mock<IStockEventSender>();

                              ICollection<StockEvent> eventsPushed = null;

                              //The last method called before sending the message to the queue needs to be mocked
                              //... and the message (here eventPushed) needs to be intercepted in the test for verification
                              mockSender.Setup(x => x.SendAsync(It.IsAny<ICollection<StockEvent>>()))
                                        .Callback((ICollection<StockEvent> events) => eventsPushed = events)
                                        .Returns(ValueTask.CompletedTask);

                              //We call the real producer to generate some real messages, which are captured above
                              var generator = new StockEventGenerator(mockSender.Object);
                              await generator.GenerateEventsAsync();

                              eventsPushed.Should().NotBeNull().And.NotBeEmpty();

                              return eventsPushed;
                          });

            /*-----------------------------------------------------
             A simple example of scenario setting.
            
             - here we chose to generate the object manually
            
               WARNING: be careful with this approach, you never
               guarantee your actual code is in sync with the
               manually generated object below.
            -----------------------------------------------------

            builder.WithMetadata(new { key = "valueKey" })
                   .WithContent(new List<Event>
                   {
                       new StockEvent
                       {
                           Name = "AAPL",
                           Price = 1.23m,
                           Timestamp = new DateTimeOffset(2022, 2, 14, 13, 14, 15, TimeSpan.Zero)
                       }
                   });
            */
        }
    }
}
