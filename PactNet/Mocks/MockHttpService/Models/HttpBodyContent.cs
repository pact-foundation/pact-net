using System;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PactNet.Configuration.Json;

namespace PactNet.Mocks.MockHttpService.Models
{
    internal class HttpBodyContent
    {
        private readonly bool _contentIsBase64Encoded;

        public dynamic Body { get; private set; }

        public string Content { get; }

        public byte[] ContentBytes
        {
            get
            {
                if (Content == null)
                {
                    return null;
                }

                return _contentIsBase64Encoded ?
                    Convert.FromBase64String(Content) :
                    Encoding.GetBytes(Content);
            }
        }

        public MediaTypeHeaderValue ContentType { get; }

        public Encoding Encoding { get; }

        private HttpBodyContent(MediaTypeHeaderValue contentType)
        {
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }

            if (contentType.CharSet == null)
            {
                throw new InvalidOperationException($"{nameof(contentType.CharSet)} must be supplied");
            }

            ContentType = contentType;
            Encoding = Encoding.GetEncoding(contentType.CharSet);
        }

        public HttpBodyContent(dynamic body, MediaTypeHeaderValue contentType) : this(contentType)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (IsJsonContentType())
            {
                Content = JsonConvert.SerializeObject(body, JsonConfig.ApiSerializerSettings);
                Body = IsJsonObjectOrArray(Content) ? body : Content;
            }
            else if (IsBinaryContentType())
            {
                if (body is byte[])
                {
                    Content = Convert.ToBase64String(body);
                    Body = body;
                    _contentIsBase64Encoded = true;
                }
                else //It's a string coming from json serialised content
                {
                    Content = Encoding.GetString(Convert.FromBase64String(body));
                    Body = body;
                }
            }
            else
            {
                Content = body.ToString();
                Body = body;
            }
        }

        public HttpBodyContent(byte[] content, MediaTypeHeaderValue contentType) : this(contentType)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var stringContent = Encoding.GetString(content);
            Content = stringContent;

            if (IsJsonContentType())
            {
                Body = IsJsonObjectOrArray(stringContent) ? JsonConvert.DeserializeObject<dynamic>(stringContent) : stringContent;
            }
            else if (IsBinaryContentType())
            {
                Body = Convert.ToBase64String(content);
            }
            else
            {
                Body = stringContent;
            }
        }

        private bool IsJsonContentType()
        {
            return ContentType.MediaType.IndexOf("application/", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                ContentType.MediaType.IndexOf("json", StringComparison.InvariantCultureIgnoreCase) > 0;
        }

        private bool IsBinaryContentType()
        {
            return ContentType.MediaType.IndexOf("application/", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                ContentType.MediaType.IndexOf("octet-stream", StringComparison.InvariantCultureIgnoreCase) > 0;
        }

        private bool IsJsonObjectOrArray(string stringContent)
        {
            var s = stringContent.Trim();

            return (s.StartsWith("{", StringComparison.InvariantCultureIgnoreCase) && s.EndsWith("}", StringComparison.InvariantCultureIgnoreCase)) ||
                (s.StartsWith("[", StringComparison.InvariantCultureIgnoreCase) && s.EndsWith("]", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}