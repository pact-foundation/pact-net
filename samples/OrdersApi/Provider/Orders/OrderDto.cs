using System;
using System.Text.Json.Serialization;

namespace Provider.Orders
{
    /// <summary>
    /// Order DTO
    /// </summary>
    /// <param name="Id">Order ID</param>
    /// <param name="Status">Order status</param>
    /// <param name="Date">Order date</param>
    public record OrderDto(int Id,
                           [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)]
                           OrderStatus Status,
                           DateTimeOffset Date);
}
