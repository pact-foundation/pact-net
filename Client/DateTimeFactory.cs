using System;

namespace Client
{
    public class DateTimeFactory
    {
        public static Func<DateTime> Now = () => DateTime.UtcNow;
    }
}
