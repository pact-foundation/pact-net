using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceResponseComparer : IProviderServiceResponseComparer
    {
        private readonly IHttpStatusCodeComparer _httpStatusCodeComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        public ProviderServiceResponseComparer()
        {
            _httpStatusCodeComparer = new HttpStatusCodeComparer();
            _httpHeaderComparer = new HttpHeaderComparer();
            _httpBodyComparer = new HttpBodyComparer();
        }

        public ComparisonResult Compare(ProviderServiceResponse expected, ProviderServiceResponse actual)
        {
            var result = new ComparisonResult();

            if (expected == null)
            {
                result.AddError("Expected response cannot be null");
                return result;
            }

            var statusResult = _httpStatusCodeComparer.Compare(expected.Status, actual.Status);
            result.AddComparisonResult(statusResult);

            if (expected.Headers != null && expected.Headers.Any())
            {
                var headerResult = _httpHeaderComparer.Compare(expected.Headers, actual.Headers);
                result.AddComparisonResult(headerResult);
            }

            if (expected.Body != null)
            {
                string expectedResponseBodyJson = JsonConvert.SerializeObject(expected.Body);
                string actualResponseBodyJson = JsonConvert.SerializeObject(actual.Body);
                var actualResponseBody = JsonConvert.DeserializeObject<JToken>(actualResponseBodyJson);
                var expectedResponseBody = JsonConvert.DeserializeObject<JToken>(expectedResponseBodyJson);

                var bodyResult = _httpBodyComparer.Compare(expectedResponseBody, actualResponseBody);
                result.AddComparisonResult(bodyResult);
            }

            return result;
        }
    }
}