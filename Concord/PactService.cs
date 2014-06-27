using System;
using Nancy;
using Nancy.Hosting.Self;

namespace Concord
{
    public class PactServiceNancyModule : NancyModule
    {
        private static HttpVerb _method;
        private static string _path;
        private static PactServiceRequest _request;
        private static PactServiceResponse _response;

        public PactServiceNancyModule()
        {
            Get[_path] = parameters => _response.Body;

            /*if (_method == HttpVerb.Get)
            {
                
            }*/
        }

        public static void Set(HttpVerb method, string path, PactServiceRequest request, PactServiceResponse response)
        {
            Reset();

            _method = method;
            _path = path;
            _request = request;
            _response = response;
        }

        private static void Reset()
        {
            _method = 0;
            _path = null;
            _request = null;
            _response = null;
        }
    }

    public class PactServiceNancyModule2 : NancyModule
    {
        public PactServiceNancyModule2()
        {
            Get["/"] = parameters => "Hello";
        }
    }

    public class PactService : IDisposable
    {
        private NancyHost _host;
        private string _description;
        private PactServiceRequest _request;
        private PactServiceResponse _response;

        public PactService(int port)
        {
            var hostConfig = new HostConfiguration
            {
                UrlReservations = { CreateAutomatically = true }
            };

            var uri = String.Format("http://localhost:{0}", port);

            _host = new NancyHost(hostConfig, new Uri(uri));
        }

        public PactService UponReceiving(string description)
        {
            _description = description;

            return this;
        }

        public PactService With(PactServiceRequest request)
        {
            _request = request;

            return this;
        }

        public PactService WillRespondWith(PactServiceResponse response)
        {
            _response = response;

            return this;
        }

        public void Start()
        {
            //Register the built module

            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();
        }

        public void Dispose()
        {
            if(_host != null)
                _host.Dispose();
        }
    }
}
