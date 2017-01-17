using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PactNet.Matchers;

namespace PactNet.Mocks.MessagingService.Consumer.Dsl
{
    public abstract class DslPart
    {
        protected DslPart _parent;
        protected string _rootName;

        protected DslPart()
            : this(null, string.Empty)
        {
        }

        protected DslPart(DslPart parent, string rootName)
        {
            _parent = parent;
            _rootName = rootName;
        }

        [JsonIgnore]
        public string Path
        {
            get
            {
                var path = string.Empty;
                if (_parent != null)
                    path = string.Format("{0}.{1}", _parent.Path, _rootName);
                else
                    path = "$.body" + _rootName;

                return path;
            }
        }
    }

    public abstract class DslPart<T> : DslPart
    {
        [JsonIgnore]
        public T Body;
        //protected Dictionary<string, IMatcher> _matchers;

        protected DslPart()
        {
        }

        protected DslPart(DslPart parent, string rootName)
            :base(parent, rootName)
        {
            //_matchers = new Dictionary<string, IMatcher>();
        }


    }
}
