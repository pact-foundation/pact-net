using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpBodyContentMapper : IHttpBodyContentMapper
    {
        private readonly IEncodingMapper _encodingMapper;

        [Obsolete("For testing only.")]
        public HttpBodyContentMapper(IEncodingMapper encodingMapper)
        {
            _encodingMapper = encodingMapper;
        }

        public HttpBodyContentMapper() : this(
            new EncodingMapper())
        {
        }

        public HttpBodyContent Convert(dynamic body, IDictionary<string, string> headers)
        {
            if (body == null)
            {
                return null;
            }

            var content = new HttpBodyContent();
            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    if (header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var contentTypeHeaderSplit = header.Value.Split(';');
                        content.ContentType = contentTypeHeaderSplit.First().Trim().ToLower();

                        var encodingString = contentTypeHeaderSplit.FirstOrDefault(x => x.Contains("charset="));
                        if (!String.IsNullOrEmpty(encodingString))
                        {
                            encodingString = encodingString.Trim().Replace("charset=", "").ToLower();
                            content.Encoding = _encodingMapper.Convert(encodingString);
                        }
                        break;
                    }
                }
            }

            content.Content = content.ContentType.Equals("application/json")
                                  ? JsonConvert.SerializeObject(body, JsonConfig.ApiSerializerSettings)
                                  : body.ToString();

            return content;
        }
    }
}