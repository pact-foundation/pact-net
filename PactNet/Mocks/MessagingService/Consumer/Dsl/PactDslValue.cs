using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    internal class PactDslValue : DslPart<string>
    {
        public PactDslValue()
            :base()
        {
        }

        public PactDslValue(DslPart parent, string rootPath, string rootName)
            :base(parent, rootPath, rootName)
        {
        }

    }
}
