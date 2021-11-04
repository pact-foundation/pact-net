using System;

namespace Consumer.Models
{
    public class UptimeResponseBody : RestResponseBody
    {
        public DateTime UpSince { get; set; }
    }
}