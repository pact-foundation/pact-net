namespace Provider.Orders
{
    /// <summary>
    /// A request to update an order
    /// </summary>
    /// <param name="Status">Updated status</param>
    public record UpdateOrderRequest(OrderStatus Status);
}
