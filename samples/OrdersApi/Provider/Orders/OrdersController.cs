using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Provider.Orders
{
    /// <summary>
    /// Orders
    /// </summary>
    [ApiController]
    [Route("/api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository orders;

        /// <summary>
        /// Initialises a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="orders">Orders repository</param>
        public OrdersController(IOrderRepository orders)
        {
            this.orders = orders;
        }

        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order</returns>
        /// <response code="200">Order</response>
        /// <response code="404">Unknown order</response>
        [HttpGet("{id}", Name = "get")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                OrderDto order = await this.orders.GetAsync(id);
                return this.Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Create a new pending order
        /// </summary>
        /// <returns>Created order</returns>
        /// <response code="201">Created order</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAsync()
        {
            int id = new Random().Next();
            var order = new OrderDto(id, OrderStatus.Pending, DateTimeOffset.Now);

            await this.orders.InsertAsync(order);

            return this.CreatedAtRoute("get", new { id = order.Id }, order);
        }

        /// <summary>
        /// Update the status of an order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="status">Updated status</param>
        /// <returns>Empty response</returns>
        /// <response code="204">Update successful</response>
        /// <response code="404">Unknown order</response>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromBody] OrderStatus status)
        {
            try
            {
                OrderDto order = await this.orders.GetAsync(id);

                await this.orders.UpdateAsync(order with { Status = status });

                return this.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return this.NotFound();
            }
        }
    }
}
