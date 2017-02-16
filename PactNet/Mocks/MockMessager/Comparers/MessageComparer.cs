using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Matchers;
using PactNet.Models.Messaging;

namespace PactNet.Mocks.MockMessager.Comparers
{
    internal class MessageComparer : IMessageComparer
    {
        private IDslPartComparer _bodyComparer;

        public MessageComparer()
        {
            _bodyComparer = new DslPartComparer();
        }

        public ComparisonResult Compare(Message expected, dynamic actual)
        {
            var result = new ComparisonResult();

            if (expected == null)
            {
                return result;
            }

            if (expected != null && actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Body is null"));
                return result;
            }

            //Need to wrap in a body root so that the expected matchingRules path lines up.

            var message = JsonConvert.SerializeObject(actual);

            JsonReader reader = new JsonTextReader(new StringReader(message));
            reader.DateParseHandling = DateParseHandling.None;

            var actualToken = new JObject();
            actualToken.Add("body", JToken.Load(reader));

            result = _bodyComparer.Compare(expected.Body, actualToken);

            return result;
        }
    }
}
