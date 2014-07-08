using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Validators
{
    public class PactProviderServiceResponseValidator : IPactProviderServiceResponseValidator
    {
        private readonly IHeaderValidator _headerValidator;
        private readonly IBodyValidator _bodyValidator;

        private const string MessagePrefix = "\t- Returns a response which";

        public PactProviderServiceResponseValidator()
        {
            _headerValidator = new HeaderValidator(MessagePrefix);
            _bodyValidator = new BodyValidator(MessagePrefix);
        }

        public void Validate(PactProviderServiceResponse expectedResponse, PactProviderServiceResponse actualResponse)
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
                _headerValidator.Validate(expectedResponse.Headers, actualResponse.Headers);
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