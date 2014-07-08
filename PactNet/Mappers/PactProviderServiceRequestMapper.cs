using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using Newtonsoft.Json;

namespace PactNet.Mappers
{
    public class PactProviderServiceRequestMapper
    {
        private static readonly IDictionary<string, HttpVerb> HttpMethodMap = new Dictionary<string, HttpVerb>
        {
            { "GET", HttpVerb.Get },
            { "POST", HttpVerb.Post },
            { "PUT", HttpVerb.Put },
            { "DELETE", HttpVerb.Delete },
            { "HEAD", HttpVerb.Head },
            { "PATCH", HttpVerb.Patch }
        };

        public PactProviderServiceRequest Convert(Request from)
        {
            if (from == null)
                return null;

            var to = new PactProviderServiceRequest
                         {
                             Method = HttpMethodMap[from.Method.ToUpper()],
                             Path = from.Path
                         };

            if (from.Headers != null && from.Headers.Any())
            {
                var fromHeaders = from.Headers.ToDictionary(x => x.Key, x => String.Join("; ", x.Value));
                to.Headers = fromHeaders;
            }

            if (from.Body != null)
            {
                to.Body = JsonConvert.DeserializeObject<dynamic>(ConvertToJsonString(from.Body));
            }

            return to;
        }

        private string ConvertToJsonString(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var body = reader.ReadToEnd();
                return body;
            }
        }
    }
}