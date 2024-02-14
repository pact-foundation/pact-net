using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using PactNet;
using PactNet.Output.Xunit;
using Xunit;
using Xunit.Abstractions;
using Match = PactNet.Matchers.Match;

namespace Consumer.Tests
{
    public class OrderCreatedConsumerTests
    {
        private readonly OrderCreatedConsumer consumer;
        private readonly Mock<IFulfilmentService> mockService;

        private readonly IMessagePactBuilderV4 pact;

        public OrderCreatedConsumerTests(ITestOutputHelper output)
        {
            this.mockService = new Mock<IFulfilmentService>();
            this.consumer = new OrderCreatedConsumer(this.mockService.Object);

            var config = new PactConfig
            {
                PactDir = "../../../pacts/",
                Outputters = new[]
                {
                    new XunitOutput(output)
                },
                DefaultJsonSettings = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                }
            };

            this.pact = Pact.V4("Fulfilment API", "Orders API", config).WithMessageInteractions();
        }

        [Fact]
        public async Task OnMessageAsync_OrderCreated_HandlesMessage()
        {
            await this.pact
                      .ExpectsToReceive("an event indicating that an order has been created")
                      .WithJsonContent(new
                      {
                          Id = Match.Integer(1)
                      })
                      .VerifyAsync<OrderCreatedEvent>(async message =>
                      {
                          await this.consumer.OnMessageAsync(message);

                          this.mockService.Verify(s => s.FulfilOrderAsync(message.Id));
                      });
        }
    }
}
