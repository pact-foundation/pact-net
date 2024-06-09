using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// Performs HTTP-based calls to the Orders API
    /// </summary>
    public class OrdersClient : IOrdersClient
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IHttpClientFactory factory;

        /// <summary>
        /// Initialises a new instance of the <see cref="OrdersClient"/> class.
        /// </summary>
        /// <param name="factory">HTTP client factory</param>
        public OrdersClient(IHttpClientFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Order</returns>
        public async Task<OrderDto> GetOrderAsync(int orderId)
        {
            using HttpClient client = this.factory.CreateClient("Orders");

            OrderDto order = await client.GetFromJsonAsync<OrderDto>($"/api/orders/{orderId}", Options);
            return order;
        }

        /// <summary>
        /// Update the status of an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="status">New status</param>
        /// <returns>Awaitable</returns>
        public async ValueTask UpdateOrderAsync(int orderId, OrderStatus status)
        {
            using HttpClient client = this.factory.CreateClient("Orders");

            await client.PutAsJsonAsync($"/api/orders/{orderId}/status", status, Options);
        }
    }
}
