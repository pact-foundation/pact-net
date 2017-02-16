using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Mocks
{
    public interface IMockMessager
    {
        void Publish(string messageTopic, string providerState, dynamic exampleMessage);

      
        dynamic GetMessageByTopic(string topic);

        dynamic GetMessageByProviderState(string providerState);

        dynamic GetMessageByTopicOrProviderState(string topic, string providerState);


    }
}
