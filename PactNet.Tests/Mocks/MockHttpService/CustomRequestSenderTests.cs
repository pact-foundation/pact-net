using System;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class CustomRequestSenderTests
    {
        private Tuple<bool, ProviderServiceRequest> _httpRequestSenderFuncCallInfo;

        private IHttpRequestSender GetSubject()
        {
            _httpRequestSenderFuncCallInfo = null;
            Func<ProviderServiceRequest, ProviderServiceResponse> httpRequestSenderFunc = request =>
            {
                _httpRequestSenderFuncCallInfo = new Tuple<bool, ProviderServiceRequest>(true, request);
                return null;
            };
            return new CustomRequestSender(httpRequestSenderFunc);
        }

        [Fact]
        public void Send_WhenCalled_InvokesTheFuncWithRequest()
        {
            var request = new ProviderServiceRequest();
            var requestSender = GetSubject();

            requestSender.Send(request);

            Assert.True(_httpRequestSenderFuncCallInfo.Item1, "httpRequestSenderFunc was called");
            Assert.Equal(request, _httpRequestSenderFuncCallInfo.Item2);
        }
    }
}
