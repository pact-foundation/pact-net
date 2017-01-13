using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderService : IMockProviderService
    {
        private readonly Func<Uri, IHttpHost> _hostFactory;
        private IHttpHost _host;
        private readonly HttpClient _httpClient;
        private readonly IHttpMethodMapper _httpMethodMapper;

        private string _providerState;
        private string _description;
        private ProviderServiceRequest _request;
        private ProviderServiceResponse _response;

        public string BaseUri { get; private set; }

        internal MockProviderService(
            Func<Uri, IHttpHost> hostFactory,
            int port,
            bool enableSsl,
            Func<string, HttpClient> httpClientFactory,
            IHttpMethodMapper httpMethodMapper)
        {
            _hostFactory = hostFactory;
            BaseUri = $"{(enableSsl ? "https" : "http")}://localhost:{port}";
            _httpClient = httpClientFactory(BaseUri);
            _httpMethodMapper = httpMethodMapper;
        }

        public MockProviderService(int port, bool enableSsl, string providerName, PactConfig config, bool bindOnAllAdapters = false)
            : this(
            baseUri => new NancyHttpHost(baseUri, providerName, config, bindOnAllAdapters),
            port,
            enableSsl,
            baseUri => new HttpClient { BaseAddress = new Uri(baseUri) },
            new HttpMethodMapper())
        {
        }

        public IMockProviderService Given(string providerState)
        {
            if (string.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            _providerState = providerState;

            return this;
        }

        public IMockProviderService UponReceiving(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Please supply a non null or empty description");
            }

            _description = description;

            return this;
        }

        public IMockProviderService With(ProviderServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException("Please supply a non null request");
            }

            if (request.Method == HttpVerb.NotSet)
            {
                throw new ArgumentException("Please supply a request Method");
            }

            if (!IsContentTypeSpecifiedForBody(request))
            {
                throw new ArgumentException("Please supply a Content-Type request header");
            }

            _request = request;

            return this;
        }

        public async Task WillRespondWith(ProviderServiceResponse response)
        {
            if (response == null)
            {
                throw new ArgumentException("Please supply a non null response");
            }

            if (response.Status <= 0)
            {
                throw new ArgumentException("Please supply a response Status");
            }

            if (!IsContentTypeSpecifiedForBody(response))
            {
                throw new ArgumentException("Please supply a Content-Type response header");
            }

            _response = response;

            await RegisterInteraction();
        }

        public void VerifyInteractions()
        {
            SendAdminHttpRequest(HttpVerb.Get, Constants.InteractionsVerificationPath);
        }

        public void Start()
        {
            StopRunningHost();
            _host = _hostFactory(new Uri(BaseUri));
            _host.Start();
        }

        public void Stop()
        {
            ClearAllState();
            StopRunningHost();
        }

        public void ClearInteractions()
        {
            if (_host != null)
            {
                SendAdminHttpRequest(HttpVerb.Delete, Constants.InteractionsPath);
            }
        }

        public async Task SendAdminHttpRequest<T>(HttpVerb method, string path, T requestContent, IDictionary<string, string> headers = null) where T : class
        {
            if (_host == null)
            {
                throw new InvalidOperationException("Unable to perform operation because the mock provider service is not running.");
            }

            var responseContent = string.Empty;

            var request = new HttpRequestMessage(_httpMethodMapper.Convert(method), path);
            request.Headers.Add(Constants.AdministrativeRequestHeaderKey, "true");

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (requestContent != null)
            {
                var requestContentJson = JsonConvert.SerializeObject(requestContent, JsonConfig.ApiSerializerSettings);
                request.Content = new StringContent(requestContentJson, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request, CancellationToken.None);
            var responseStatusCode = response.StatusCode;

            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }

            Dispose(request);
            Dispose(response);

            if (responseStatusCode != HttpStatusCode.OK)
            {
                throw new PactFailureException(responseContent);
            }
        }

        private async Task RegisterInteraction()
        {
            if (string.IsNullOrEmpty(_description))
            {
                throw new InvalidOperationException("description has not been set, please supply using the UponReceiving method.");
            }

            if (_request == null)
            {
                throw new InvalidOperationException("request has not been set, please supply using the With method.");
            }

            if (_response == null)
            {
                throw new InvalidOperationException("response has not been set, please supply using the WillRespondWith method.");
            }

            var interaction = new ProviderServiceInteraction
            {
                ProviderState = _providerState,
                Description = _description,
                Request = _request,
                Response = _response
            };

            var testContext = BuildTestContext();

            await SendAdminHttpRequest(HttpVerb.Post, Constants.InteractionsPath, interaction, new Dictionary<string, string> { { Constants.AdministrativeRequestTestContextHeaderKey, testContext } });

            ClearTrasientState();
        }

        private static string BuildTestContext()
        {
            var stack = new StackTrace(true);
            var stackFrames = stack.GetFrames() ?? new StackFrame[0];

            var relevantStackFrameSummaries = new List<string>();

            foreach (var stackFrame in stackFrames)
            {
                var type = stackFrame.GetMethod().ReflectedType;

                if (type == null ||
                    (type.Assembly.GetName().Name.StartsWith("PactNet", StringComparison.CurrentCultureIgnoreCase) &&
                    !type.Assembly.GetName().Name.Equals("PactNet.Tests", StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }

                //Don't care about any mscorlib frames down
                if (type.Assembly.GetName().Name.Equals("mscorlib", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }

                relevantStackFrameSummaries.Add($"{type.Name}.{stackFrame.GetMethod().Name}");
            }

            return string.Join(" ", relevantStackFrameSummaries);
        }

        private async void SendAdminHttpRequest(HttpVerb method, string path)
        {
            await SendAdminHttpRequest<object>(method, path, null);
        }

        private void StopRunningHost()
        {
            if (_host != null)
            {
                _host.Stop();
                _host = null;
            }
        }

        private void ClearAllState()
        {
            ClearTrasientState();
            ClearInteractions();
        }

        private void ClearTrasientState()
        {
            _request = null;
            _response = null;
            _providerState = null;
            _description = null;
        }

        private bool IsContentTypeSpecifiedForBody(IHttpMessage message)
        {
            //No content-type required if there is no body
            if (message.Body == null)
            {
                return true;
            }

            IDictionary<string, string> headers = null;
            if (message.Headers != null)
            {
                headers = new Dictionary<string, string>(message.Headers, StringComparer.InvariantCultureIgnoreCase);
            }

            return headers != null && headers.ContainsKey("Content-Type");
        }

        private void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
