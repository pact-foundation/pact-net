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

        public void Compare(ProviderServiceResponse response1, ProviderServiceResponse response2)
        {
            if (response1 == null)
            {
                _reporter.ReportError("Expected response cannot be null");
                return;
            }

            _reporter.ReportInfo(String.Format("{0} has status code of {1}", MessagePrefix, response1.Status));
            if (!response1.Status.Equals(response2.Status))
            {
                _reporter.ReportError(expected: response1.Status, actual: response2.Status);
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