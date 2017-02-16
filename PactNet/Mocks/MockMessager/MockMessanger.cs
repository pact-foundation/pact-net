using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PactNet.Mocks.MockMessager
{
    public class SampleMessage
    {
        public string Description { get; set; }
        public string ProivderState { get; set; }
        public dynamic ExampleMessage { get; set; }
    }

    public class MockMessanger : IMockMessager
    {
       
        private readonly List<SampleMessage> exampleMessages;

        public MockMessanger()
        {
            exampleMessages = new List<SampleMessage>();
        }

        public dynamic GetMessageByProviderState(string providerState)
        {
            var message = this.exampleMessages.FirstOrDefault(x => string.Compare(x.ProivderState, providerState, true, CultureInfo.InvariantCulture) == 0);

            if (message != null)
                return message.ExampleMessage;
            else
                return null;
        }

        public dynamic GetMessageByTopic(string topic)
        {
            var message = this.exampleMessages.FirstOrDefault(x => string.Compare(x.Description, topic, true, CultureInfo.InvariantCulture) == 0);

            if (message != null)
                return message.ExampleMessage;
            else
                return null;

        }

        public dynamic GetMessageByTopicOrProviderState(string topic, string providerState)
        {
            var message = this.exampleMessages.FirstOrDefault(x =>
             string.Compare(x.Description, topic, true, CultureInfo.InvariantCulture) == 0 ||
                string.Compare(x.ProivderState, providerState, true, CultureInfo.InvariantCulture) == 0);

            if (message != null)
                return message.ExampleMessage;
            else
                return null;
        }

        public void Publish(string messageTopic, string providerState, dynamic exampleMessage)
        {
            SampleMessage example = new SampleMessage()
            {
                Description = messageTopic,
                ProivderState = providerState,
                ExampleMessage = exampleMessage
            };

            this.exampleMessages.Any(x =>
                string.Compare(x.Description, messageTopic, true, CultureInfo.InvariantCulture) == 0 &&
                string.Compare(x.ProivderState, providerState, true, CultureInfo.InvariantCulture) == 0);
         
            this.exampleMessages.Add(example);
        }
    }
}
