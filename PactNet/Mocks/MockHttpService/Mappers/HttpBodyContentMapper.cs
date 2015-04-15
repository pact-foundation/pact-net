using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal class HttpBodyContentMapper : IHttpBodyContentMapper
    {
        private readonly IEncodingMapper _encodingMapper;

        internal HttpBodyContentMapper(IEncodingMapper encodingMapper)
        {
            _encodingMapper = encodingMapper;
        }

        public HttpBodyContentMapper() : this(new EncodingMapper())
        {
        }

        public HttpBodyContent Convert(dynamic body, IDictionary<string, string> headers)
        {
            if (body == null)
            {
                return null;
            }

            var contentInfo = ParseContentHeaders(headers);

            return new HttpBodyContent(body, contentInfo.Item1, contentInfo.Item2);
        }

        public HttpBodyContent Convert(string content, IDictionary<string, string> headers)
        {
            if (content == null)
            {
                return null;
            }

            var contentInfo = ParseContentHeaders(headers);

            return new HttpBodyContent(content, contentInfo.Item1, contentInfo.Item2);
        }

        private Tuple<string, Encoding> ParseContentHeaders(IDictionary<string, string> headers)
        {
            string contentType = null;
            Encoding encoding = null;

            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    if (header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var contentTypeHeaderSplit = header.Value.Split(';');
                        contentType = contentTypeHeaderSplit.First().Trim().ToLower();

                        var encodingString = contentTypeHeaderSplit.FirstOrDefault(x => x.Contains("charset="));
                        if (!String.IsNullOrEmpty(encodingString))
                        {
                            encodingString = encodingString.Trim().Replace("charset=", String.Empty).ToLower();
                            encoding = _encodingMapper.Convert(encodingString);
                        }
                        break;
                    }
                }
            }

            return new Tuple<string, Encoding>(contentType, encoding);
        }
    }
}