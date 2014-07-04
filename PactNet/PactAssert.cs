using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Configuration.Json;
using PactNet.Validators;

namespace PactNet
{
    public class PactAssert
    {
        private readonly IBodyValidator _bodyValidator;
        private const string MessagePrefix = "\t- Returns a response which";

        public PactAssert()
        {
            _bodyValidator = new BodyValidator(MessagePrefix);
        }

        public void Equal(PactProviderResponse expectedResponse, PactProviderResponse actualResponse)
        {
            if (expectedResponse == null)
            {
                throw new PactAssertException("Expected response cannot be null");
            }

            Console.WriteLine("{0} has status code of {1}", MessagePrefix, expectedResponse.Status);
            if (!expectedResponse.Status.Equals(actualResponse.Status))
            {
                throw new PactAssertException(expectedResponse.Status, actualResponse.Status);
            }

            if (expectedResponse.Headers != null && expectedResponse.Headers.Any())
            {
                if (actualResponse.Headers == null)
                {
                    throw new PactAssertException("Headers are null in response");
                }

                foreach (var header in expectedResponse.Headers)
                {
                    Console.WriteLine("{0} includes header {1} with value {2}", MessagePrefix, header.Key, header.Value);

                    string headerValue;

                    if (actualResponse.Headers.TryGetValue(header.Key, out headerValue))
                    {
                        if (!header.Value.Equals(headerValue))
                        {
                            throw new PactAssertException(header.Value, headerValue);
                        }
                    }
                    else
                    {
                        throw new PactAssertException("Header does not exist in response");
                    }
                }
            }

            if (expectedResponse.Body != null)
            {
                string expectedResponseBodyJson = JsonConvert.SerializeObject(expectedResponse.Body);
                string actualResponseBodyJson = JsonConvert.SerializeObject(actualResponse.Body);
                var actualResponseBody = JsonConvert.DeserializeObject<JToken>(actualResponseBodyJson);
                var expectedResponseBody = JsonConvert.DeserializeObject<JToken>(expectedResponseBodyJson);

                _bodyValidator.Validate(expectedResponseBody, actualResponseBody);
            }
        }
    }
}