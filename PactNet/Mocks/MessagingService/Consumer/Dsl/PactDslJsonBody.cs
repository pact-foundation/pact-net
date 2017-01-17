using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Matchers;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    internal class PactDslJsonBody : DslPart<Dictionary<string, DslPart>>
    {

        public PactDslJsonBody()
            :base()
        {
            _body = new Dictionary<string, DslPart>();
        }

        public PactDslJsonBody(DslPart parent, string rootPath, string rootName)
            :base(parent, rootPath, rootName)
        {
            _body = new Dictionary<string, DslPart>();
        }

        public DslPart Object(string name)
        {
            this._body["name"] = new PactDslJsonBody(this, string.Format("{0}/{1}", _rootPath, name), name);
            return this._body["name"];
        }

        public DslPart StringType(string name, string example)
        {
            this._body["name"] = new PactDslValue(this, name, example);
            return this._body["name"];
        }

    }
}
