using System;
using System.Collections.Generic;
using System.Linq;

namespace PactNet.Models
{
    public class ProviderStates
    {
        public Action SetUp { get; private set; }
        public Action TearDown { get; private set; }

        private List<ProviderState> _providerStates;

        public ProviderStates(Action setUp = null, Action tearDown = null)
        {
            SetUp = setUp;
            TearDown = tearDown;
        }

        public void Add(ProviderState providerState)
        {
            //TODO: Ensure no duplicate provider states

            _providerStates = _providerStates ?? new List<ProviderState>();

            _providerStates.Add(providerState);
        }

        public ProviderState Find(string providerState)
        {
            if (String.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            if (_providerStates != null && _providerStates.Any(x => x.ProviderStateDescription == providerState))
            {
                return _providerStates.FirstOrDefault(x => x.ProviderStateDescription == providerState);
            }

            return null;
        }
    }
}