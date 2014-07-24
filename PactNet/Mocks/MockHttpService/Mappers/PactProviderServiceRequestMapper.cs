using System;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class PactProviderServiceRequestMapper : IPactProviderServiceRequestMapper
    {
        private readonly IHttpVerbMapper _httpVerbMapper;
        private readonly IHttpBodyContentMapper _httpBodyContentMapper;

        [Obsolete("For testing only.")]
        public PactProviderServiceRequestMapper(
            IHttpVerbMapper httpVerbMapper,
            IHttpBodyContentMapper httpBodyContentMapper)
        {
            _httpVerbMapper = httpVerbMapper;
            _httpBodyContentMapper = httpBodyContentMapper;
        }

        public PactProviderServiceRequestMapper() : this(
            new HttpVerbMapper(),
            new HttpBodyContentMapper())
        {
        }

        public PactProviderServiceRequest Convert(Request from)
        {
            if (from == null)
            {
                return null;
            }
                
            var httpVerb = _httpVerbMapper.Convert(from.Method.ToUpper());

            var to = new PactProviderServiceRequest
            {
                Method = httpVerb,
                Path = from.Path,
                Query = !String.IsNullOrEmpty(from.Url.Query) ? from.Url.Query.TrimStart('?') : null
            };

            if (from.Headers != null && from.Headers.Any())
            {
                var fromHeaders = from.Headers.ToDictionary(x => x.Key, x => String.Join(", ", x.Value));
                to.Headers = fromHeaders;
            }

            if (from.Body != null && from.Body.Length > 0)
            {
                var content = ConvertStreamToString(from.Body);
                var httpBodyContent = _httpBodyContentMapper.Convert(content, to.Headers);

                to.Body = httpBodyContent.Body;
            }

            return to;
        }

        private string ConvertStreamToString(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                return body;
            }
        }
    }
}