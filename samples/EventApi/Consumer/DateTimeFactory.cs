using System;

namespace Consumer
{
    public class DateTimeFactory
    {
        public static DateTime? Override { get; set; } = null;

        public static DateTime Now()
        {
            return Override ?? DateTime.Now;
        }
    }
}
