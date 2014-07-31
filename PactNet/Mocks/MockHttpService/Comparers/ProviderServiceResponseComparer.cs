using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceResponseComparer : IProviderServiceResponseComparer
    {
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        private const string MessagePrefix = "\t- Returns a response which";

        public ProviderServiceResponseComparer()
        {
            _httpHeaderComparer = new HttpHeaderComparer(MessagePrefix);
            _httpBodyComparer = new HttpBodyComparer(MessagePrefix);
        }

        public void Compare(ProviderServiceResponse response1, ProviderServiceResponse response2)
        {
            if (response1 == null)
            {
                throw new CompareFailedException("Expected response cannot be null");
            }

            Console.WriteLine("{0} has status code of {1}", MessagePrefix, response1.Status);
            if (!response1.Status.Equals(response2.Status))
            {
                throw new CompareFailedException(response1.Status, response2.Status);
            }

            if (response1.Headers != null && response1.Headers.Any())
            {
                _httpHeaderComparer.Compare(response1.Headers, response2.Headers);
            }

            if (response1.Body != null)
            {
                string expectedResponseBodyJson = JsonConvert.SerializeObject(response1.Body);
                string actualResponseBodyJson = JsonConvert.SerializeObject(response2.Body);
                var actualResponseBody = JsonConvert.DeserializeObject<JToken>(actualResponseBodyJson);
                var expectedResponseBody = JsonConvert.DeserializeObject<JToken>(expectedResponseBodyJson);

                _httpBodyComparer.Validate(expectedResponseBody, actualResponseBody);
            }
        }
    }
}