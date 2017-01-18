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
        protected Dictionary<string, IMatcher> _matchers;

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

        [JsonIgnore]
        public string Name { get { return _rootName; } }

        [JsonIgnore]
        public virtual string Path
        {
            get
            {
                var path = string.Empty;
                if (_parent != null)
                    if (!string.IsNullOrEmpty(_rootName))
                        path = string.Format("{0}.{1}", _parent.Path, _rootName);
                    else
                        path = _parent.Path;
                else
                    path = "$.body" + _rootName;

                return path;
            }
        }

        public abstract Dictionary<string, List<IMatcher>> Matchers { get; }
        public abstract Dictionary<string, object> Content { get; }

        [JsonIgnore]
        public abstract object Value { get; }

        [JsonIgnore]
        public abstract bool IsPrimitive { get; }

        protected DslPart MatchType()
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
