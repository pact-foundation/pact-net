using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public void Validate(PactProviderRequest expectedRequest, Request actualRequest)
        {
            if (expectedRequest == null)
            {
                throw new PactAssertException("Expected request cannot be null");
            }

            var expectedRequestMethod = expectedRequest.Method.ToString().ToLower();
            var actualRequestMethod = actualRequest.Method.ToLower();

            Console.WriteLine("{0} has method set to {1}", MessagePrefix, expectedRequestMethod);
            if (!expectedRequestMethod.Equals(actualRequestMethod))
            {
                throw new PactAssertException(expectedRequestMethod, actualRequestMethod);
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

                var actualRequestHeaders = new Dictionary<string, string>();
                if (actualRequest.Headers != null && actualRequest.Headers.Any())
                {
                    actualRequestHeaders = actualRequest.Headers.ToDictionary(x => x.Key, x => String.Join("; ", x.Value));
                }

                _headerValidator.Validate(expectedRequest.Headers, actualRequestHeaders);
            }

            if (expectedRequest.Body != null)
            {
                string expectedRequestBodyJson = JsonConvert.SerializeObject(expectedRequest.Body);

                var actualRequestBody = JsonConvert.DeserializeObject<JToken>(ConvertToJsonString(actualRequest.Body));
                var expectedRequestBody = JsonConvert.DeserializeObject<JToken>(expectedRequestBodyJson);

                _bodyValidator.Validate(actualRequestBody, expectedRequestBody);
            }
        }

        private string ConvertToJsonString(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                return body;
            }
        }

    }
}