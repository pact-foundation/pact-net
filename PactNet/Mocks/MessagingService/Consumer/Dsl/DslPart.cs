using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Matchers;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    internal abstract class DslPart
    {
        
    }

    internal abstract class DslPart<T> : DslPart
    {
        protected DslPart _parent;
        protected string _rootPath;
        protected string _rootName;

        protected T _body;
        protected Dictionary<string, IMatcher> _matchers;

        protected DslPart()
            :this(null,".",string.Empty)
        {
        }

        protected DslPart(DslPart parent, string rootPath, string rootName)
        {
            _parent = parent;
            _rootPath = rootPath;
            _rootName = rootName;

            _matchers = new Dictionary<string, IMatcher>();
        }

    }
}
