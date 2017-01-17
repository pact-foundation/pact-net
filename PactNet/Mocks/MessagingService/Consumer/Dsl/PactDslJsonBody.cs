using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Matchers;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    public class PactDslJsonBody : DslPart<Dictionary<string, DslPart>>
    {

        public PactDslJsonBody()
            :base()
        {
            Body = new Dictionary<string, DslPart>();
        }

        public PactDslJsonBody(DslPart parent, string rootName)
            :base(parent, rootName)
        {
            Body = new Dictionary<string, DslPart>();
        }

        public PactDslJsonBody Object(string name)
        {
            Body[name] = new PactDslJsonBody(this, name);
            return (PactDslJsonBody)Body[name];
        }

        public PactDslJsonBody CloseObject()
        {
            if (_parent != null)
                return (PactDslJsonBody)_parent;

            return this;
        }

        public PactDslJsonBody StringValue(string name, string example)
        {
            Body[name] = new PactDslValue<string>(this, name, example);
            return this;
        }

        public PactDslJsonBody Int32Value(string name, int example)
        {
            Body[name] = new PactDslValue<int>(this, name, example);
            return this;
        }


    }
}
