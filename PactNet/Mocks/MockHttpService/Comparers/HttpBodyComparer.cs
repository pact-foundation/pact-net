using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Mocks.MockHttpService.Comparers
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
            if (body1 == null)
            {
                return;
            }

            if (body1 != null && body2 == null)
            {
                throw new CompareFailedException("Body is null");
            }

            string body1Json = JsonConvert.SerializeObject(body1);
            string body2Json = JsonConvert.SerializeObject(body2);
            var httpBody1 = JsonConvert.DeserializeObject<JToken>(body1Json);
            var httpBody2 = JsonConvert.DeserializeObject<JToken>(body2Json);

            if (httpBody1 == null)
            {
                return;
            }

            if (httpBody1 != null && httpBody2 == null)
            {
                throw new CompareFailedException("Body is null");
            }

            if (httpBody1.Type == JTokenType.Array)
            {
                Console.WriteLine("{0} has a matching body", _messagePrefix);

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
                    throw new CompareFailedException(httpBody1, httpBody2);
                }
            }
            else if (httpBody1.Type == JTokenType.String)
            {
                if (!httpBody1.Equals(httpBody2))
                {
                    throw new CompareFailedException(httpBody1, httpBody2);
                }
            }
            else
            {
                if (!JToken.DeepEquals(httpBody1, httpBody2))
                {
                    throw new CompareFailedException(httpBody1, httpBody2);
                }
            }
        }

        private static void AssertPropertyValuesMatch(JToken httpBody1, JToken httpBody2)
        {
            foreach (var item1 in httpBody1)
            {
                var item2 = httpBody2.FirstOrDefault(x => x.Path == item1.Path);

                if (item2 == null)
                {
                    throw new CompareFailedException(String.Format("Body.{0} does not exist", item1.Path));
                }

                if(!JToken.DeepEquals(item1, item2))
                {
                    throw new CompareFailedException(String.Format("Body.{0}", item1.Path), item1, item2);
                }
            }
        }
    }
}