using PactNet.Mocks.MockHttpService.Models;
using System;
using System.Linq;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class ProviderServiceRequestMapper : IProviderServiceRequestMapper
    {
        private readonly IHttpVerbMapper _httpVerbMapper;
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        internal ProviderServiceRequestMapper(
            IHttpVerbMapper httpVerbMapper,
            IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpVerbMapper = httpVerbMapper;
            _httpBodyContentMapper = httpBodyContentMapper;
        }

        public ProviderServiceRequestMapper() : this(
            new HttpVerbMapper(),
            new HttpBodyContentMapper())
        {
        }

        public ProviderServiceRequest Convert(IRequestWrapper from)
        {
            if (from == null)
            {
                return null;
            }

            var httpVerb = _httpVerbMapper.Convert(from.Method.ToUpper());

            var to = new ProviderServiceRequest
            {
                Method = httpVerb,
                Path = from.Path,
                Query = !String.IsNullOrEmpty(from.Query) ? from.Query.TrimStart('?') : null
            };

            if (from.Headers != null && from.Headers.Any())
            {
                var fromHeaders = from.Headers.ToDictionary(x => x.Key, x => String.Join(", ", x.Value));
                to.Headers = fromHeaders;
            }

            if (from.Body != null && from.Body.Length > 0)
            {
                var httpBodyContent = _httpBodyContentMapper.Convert(new BinaryContentMapRequest { Content = from.Body, Headers = to.Headers });

                to.Body = httpBodyContent.Body;
            }

            return to;
        }
    }
}