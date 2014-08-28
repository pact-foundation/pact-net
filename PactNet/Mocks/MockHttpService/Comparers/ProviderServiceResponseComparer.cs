using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceResponseComparer : IProviderServiceResponseComparer
    {
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;
        private readonly IReporter _reporter;

        private const string MessagePrefix = "\t- Returns a response which";

        public ProviderServiceResponseComparer(IReporter reporter)
        {
            _reporter = reporter;
            _httpHeaderComparer = new HttpHeaderComparer(MessagePrefix, _reporter);
            _httpBodyComparer = new HttpBodyComparer(MessagePrefix, _reporter); //TODO: MessagePrefix isn't real nice
        }

        public void Compare(ProviderServiceResponse expected, ProviderServiceResponse actual)
        {
            if (expected == null)
            {
                _reporter.ReportError("Expected response cannot be null");
                return;
            }

            _reporter.ReportInfo(String.Format("{0} has status code of {1}", MessagePrefix, expected.Status));
            if (!expected.Status.Equals(actual.Status))
            {
                _reporter.ReportError(expected: expected.Status, actual: actual.Status);
            }

            if (expected.Headers != null && expected.Headers.Any())
            {
                _httpHeaderComparer.Compare(expected.Headers, actual.Headers);
            }

            if (expected.Body != null)
            {
                string expectedResponseBodyJson = JsonConvert.SerializeObject(expected.Body);
                string actualResponseBodyJson = JsonConvert.SerializeObject(actual.Body);
                var actualResponseBody = JsonConvert.DeserializeObject<JToken>(actualResponseBodyJson);
                var expectedResponseBody = JsonConvert.DeserializeObject<JToken>(expectedResponseBodyJson);

                _httpBodyComparer.Validate(expectedResponseBody, actualResponseBody);
            }
        }
    }
}