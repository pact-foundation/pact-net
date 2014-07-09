using Nancy.Hosting.Self;

namespace PactNet.Mocks.MockHttpService.Configuration
{
    public class NancyConfig
    {
        private static HostConfiguration _hostConfiguration;
        public static HostConfiguration HostConfiguration
        {
            get
            {
                _hostConfiguration = _hostConfiguration ?? new HostConfiguration
                {
                    UrlReservations =
                    {
                        CreateAutomatically = true
                    },
                    AllowChunkedEncoding = false
                };
                return _hostConfiguration;
            }
        }
    }
}
