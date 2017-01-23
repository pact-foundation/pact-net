using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Models.Messaging.Consumer.Dsl
{
    public class PactDslJsonRoot : PactDslJsonBody
    {
        public PactDslJsonRoot()
            :base("body")
        {
        }
    }
}
