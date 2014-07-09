using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Comparers
{
    public class HttpBodyComparer : IHttpBodyComparer
    {
        private readonly string _messagePrefix;

        public HttpBodyComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Validate(dynamic body1, dynamic body2)
        {
            string body1Json = JsonConvert.SerializeObject(body1);
            string body2Json = JsonConvert.SerializeObject(body2);
            var httpBody1 = JsonConvert.DeserializeObject<JToken>(body1Json);
            var httpBody2 = JsonConvert.DeserializeObject<JToken>(body2Json);

            if (httpBody1.Type == JTokenType.Array)
            {
                Console.WriteLine("{0} has a matching body", _messagePrefix);

                if (httpBody2 == null || !httpBody2.Any())
                {
                    throw new PactComparisonFailed("Body is null or empty");
                }

                for (var i = 0; i < httpBody1.Count(); i++)
                {
                    var body1Object = httpBody1[i];
                    var body2Object = httpBody2[i];

                    AssertPropertyValuesMatch(body1Object, body2Object);
                }
            }
            else if (httpBody1.Type == JTokenType.Object)
            {
                AssertPropertyValuesMatch(httpBody1, httpBody2);
            }
            else if (httpBody1.Type == JTokenType.Integer)
            {
                if (!httpBody1.Equals(httpBody2))
                {
                    throw new PactComparisonFailed(httpBody1, httpBody2);
                }
            }
            else if (httpBody1.Type == JTokenType.String)
            {
                if (!httpBody1.Equals(httpBody2))
                {
                    throw new PactComparisonFailed(httpBody1, httpBody2);
                }
            }
            else
            {
                throw new NotSupportedException("Type has not been implemented for comparison");
            }
        }

        private static void AssertPropertyValuesMatch(JToken httpBody1, JToken httpBody2)
        {
            if (httpBody2 == null || !httpBody2.Any())
            {
                throw new PactComparisonFailed("Body is null");
            }

            foreach (var leftItem in httpBody1)
            {
                var rightItem = httpBody2.FirstOrDefault(x => x.Path == leftItem.Path);

                if (rightItem == null)
                {
                    throw new PactComparisonFailed(String.Format("Body.{0} does not exist", leftItem.Path));
                }

                //TODO: Work on these comparisons
                if(!JToken.DeepEquals(leftItem, rightItem))
                {
                    throw new PactComparisonFailed(String.Format("Body.{0}", leftItem.Path), leftItem, rightItem);
                }
            }
        }
    }
}