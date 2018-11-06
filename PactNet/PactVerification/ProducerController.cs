using System;
using System.Collections.Generic;

namespace PactNet.PactVerification
{
    public class ProducerController : IProducerHttpProxy
    {
        private readonly IMessageInvoker _messageInvoker;

        public ProducerController(IDictionary<string, Action> providerStates, IDictionary<string, Func<string>> messagePublishers)
            : this(new MessageInvoker(providerStates, messagePublishers))
        {
        }

        internal ProducerController(IMessageInvoker messageInvoker)
        {
            _messageInvoker = messageInvoker;
        }

        public string Invoke(PactMessageDescription messageDescription)
        {
            return _messageInvoker.Invoke(messageDescription);
        }
    }
}
