using System.Collections.Generic;

namespace Provider.Api.Web.Models
{
    public class Message<T>
    {
        public T Contents { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
