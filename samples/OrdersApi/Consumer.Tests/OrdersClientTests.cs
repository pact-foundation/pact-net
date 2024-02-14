using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PactNet;
using PactNet.Output.Xunit;
using Xunit;
using Xunit.Abstractions;
using Match = PactNet.Matchers.Match;

namespace Consumer.Tests
{
    public class OrdersClientTests
    {
        private readonly IPactBuilderV4 pact;
        private readonly Mock<IHttpClientFactory> mockFactory;

        public OrdersClientTests(ITestOutputHelper output)
        {
            this.mockFactory = new Mock<IHttpClientFactory>();

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
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                LogLevel = PactLogLevel.Debug
            };

            this.pact = Pact.V4("Fulfilment API", "Orders API", config).WithHttpInteractions();
        }

        [Fact]
        public async Task GetOrderAsync_WhenCalled_ReturnsOrder()
        {
            var expected = new OrderDto(1, OrderStatus.Pending, new DateTimeOffset(2023, 6, 28, 12, 13, 14, TimeSpan.FromHours(1)));

            this.pact
                .UponReceiving("a request for an order by ID")
                    .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "1" })
                    .WithRequest(HttpMethod.Get, "/api/orders/1")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithJsonBody(new
                    {
                        Id = Match.Integer(expected.Id),
                        Status = Match.Regex(expected.Status.ToString(), string.Join("|", Enum.GetNames<OrderStatus>())),
                        Date = Match.Type(expected.Date.ToString("O"))
                    });

            await this.pact.VerifyAsync(async ctx =>
            {
                this.mockFactory
                    .Setup(f => f.CreateClient("Orders"))
                    .Returns(() => new HttpClient
                    {
                        BaseAddress = ctx.MockServerUri,
                        DefaultRequestHeaders =
                        {
                            Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }
                        }
                    });

                var client = new OrdersClient(this.mockFactory.Object);

                OrderDto order = await client.GetOrderAsync(1);

                order.Should().Be(expected);
            });
        }

        [Fact]
        public async Task GetOrderAsync_UnknownOrder_ReturnsNotFound()
        {
            this.pact
                .UponReceiving("a request for an order with an unknown ID")
                    .WithRequest(HttpMethod.Get, "/api/orders/404")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NotFound);

            await this.pact.VerifyAsync(async ctx =>
            {
                this.mockFactory
                    .Setup(f => f.CreateClient("Orders"))
                    .Returns(() => new HttpClient
                    {
                        BaseAddress = ctx.MockServerUri,
                        DefaultRequestHeaders =
                        {
                            Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") }
                        }
                    });

                var client = new OrdersClient(this.mockFactory.Object);

                Func<Task> action = () => client.GetOrderAsync(404);

                var response = await action.Should().ThrowAsync<HttpRequestException>();
                response.And.StatusCode.Should().Be(HttpStatusCode.NotFound);
            });
        }

        [Fact]
        public async Task UpdateOrderAsync_WhenCalled_UpdatesOrder()
        {
            this.pact
                .UponReceiving("a request to update the status of an order")
                    .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "1" })
                    .WithRequest(HttpMethod.Put, "/api/orders/1/status")
                    .WithJsonBody(Match.Regex(OrderStatus.Fulfilling.ToString(), string.Join("|", Enum.GetNames<OrderStatus>())))
                .WillRespond()
                    .WithStatus(HttpStatusCode.NoContent);

            await this.pact.VerifyAsync(async ctx =>
            {
                this.mockFactory
                    .Setup(f => f.CreateClient("Orders"))
                    .Returns(() => new HttpClient
                    {
                        BaseAddress = ctx.MockServerUri
                    });

                var client = new OrdersClient(this.mockFactory.Object);

                await client.UpdateOrderAsync(1, OrderStatus.Fulfilling);
            });
        }
    }
}
