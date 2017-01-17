using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    public class PactDslValue<T> : DslPart<T> where T : IConvertible
    {
        public PactDslValue()
            :base()
        {
        }

        public Dictionary<string, T> Value;

        public PactDslValue(DslPart parent, string rootName, T value)
            :base(parent, rootName)
        {
            Body = value;
            this.Value = new Dictionary<string, T> {{rootName,value}};
        }

    }
}
