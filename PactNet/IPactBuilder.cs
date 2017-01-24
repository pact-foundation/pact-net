using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Models.Messaging;
using System.Collections;
using System.Collections.Generic;

namespace PactNet
{
    public interface IPactBuilder
    {
        IPactBuilder ServiceConsumer(string consumerName);
        IPactBuilder HasPactWith(string providerName);
        void Build();
    }

    public interface IPactHttpServiceBuilder : IPactBuilder
    {
        IMockProviderService MockService(int port, bool enableSsl = false, bool bindOnAllAdapters = false);
        IMockProviderService MockService(int port, JsonSerializerSettings jsonSerializerSettings, bool enableSsl = false, bool bindOnAllAdapters = false);        
    }

    public interface IPactMessagingBuilder : IPactBuilder
    {
        IPactMessagingBuilder WithContent(Message message);
        IPactMessagingBuilder WithMetaData(Dictionary<string, object> metaData);
    }
}