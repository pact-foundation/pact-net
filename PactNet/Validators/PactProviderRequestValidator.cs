using System;
using System.Linq;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Mappers;

namespace PactNet.Validators
{
    //TODO: This will need to be refactored
    public class PactProviderRequestValidator : IPactProviderRequestValidator
    {
        private readonly IHeaderValidator _headerValidator;
        private readonly IBodyValidator _bodyValidator;

        private const string MessagePrefix = "\t- Request";

        public PactProviderRequestValidator()
        {
            _headerValidator = new HeaderValidator(MessagePrefix);
            _bodyValidator = new BodyValidator(MessagePrefix);
        }

        public void Validate(PactProviderRequest expectedRequest, PactProviderRequest actualRequest)
        {
            if (expectedRequest == null)
            {
                throw new PactAssertException("Expected request cannot be null");
            }

            Console.WriteLine("{0} has method set to {1}", MessagePrefix, expectedRequest.Method);
            if (!expectedRequest.Method.Equals(actualRequest.Method))
            {
                throw new PactAssertException(expectedRequest.Method, actualRequest.Method);
            }

            Console.WriteLine("{0} has path set to {1}", MessagePrefix, expectedRequest.Path);
            if (!expectedRequest.Path.Equals(actualRequest.Path))
            {
                throw new PactAssertException(expectedRequest.Path, actualRequest.Path);
            }

            if (expectedRequest.Headers != null && expectedRequest.Headers.Any())
            {
                if (actualRequest.Headers == null)
                {
                    throw new PactAssertException("Headers are null");
                }

                _headerValidator.Validate(expectedRequest.Headers, actualRequest.Headers);
            }

            if (expectedRequest.Body != null)
            {
                string expectedRequestBodyJson = JsonConvert.SerializeObject(expectedRequest.Body);
                string actualRequestBodyJson = JsonConvert.SerializeObject(actualRequest.Body);


                var actualRequestBody = JsonConvert.DeserializeObject<JToken>(actualRequestBodyJson);
                var expectedRequestBody = JsonConvert.DeserializeObject<JToken>(expectedRequestBodyJson);

                _bodyValidator.Validate(actualRequestBody, expectedRequestBody);
            }
        }
    }
}