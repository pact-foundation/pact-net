namespace Consumer
{
    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// The order is pending fulfilment
        /// </summary>
        Pending,

        /// <summary>
        /// The order is being fulfilled
        /// </summary>
        Fulfilling,

        /// <summary>
        /// The order has been shipped
        /// </summary>
        Shipped
    }
}
