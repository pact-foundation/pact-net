using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService.Matchers.Regex;
using PactNet.Mocks.MockHttpService.Matchers.Type;

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

            _matchers = new Dictionary<string, IMatcher>();
        }

        protected Dictionary<string, IMatcher> _matchers;
        
        public Dictionary<string, IMatcher> Matchers
        {
            get { return _matchers; }
            set { _matchers = value; }
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

        protected DslPart MatchType(string type)
        {
            _matchers["type"] = new TypeMatcher();
            return this;
        }

        protected DslPart MatchRegex(string regex)
        {
            _matchers["regex"] = new RegexMatcher(regex);
            return this;
        }

        //TODO: add more matchers
    }

    public abstract class DslPart<T> : DslPart
    {
        [JsonIgnore]
        public T Body;

        protected DslPart()
        {
        }

        protected DslPart(DslPart parent, string rootName)
            :base(parent, rootName)
        {
        }

    }
}
