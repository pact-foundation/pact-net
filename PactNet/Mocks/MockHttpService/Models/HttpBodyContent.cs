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
                Body = IsJsonString(Content) ? Content : body;
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
                Body = IsJsonString(stringContent) ? stringContent : JsonConvert.DeserializeObject<dynamic>(stringContent);
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

        private bool IsJsonString(string stringContent)
        {
            //Json string data types get serialized/deserialized differently than other non-string types,
            //and so we have to make sure to preserve the extra double-quotes.
            //Serialization Guide: http://www.newtonsoft.com/json/help/html/SerializationGuide.htm
            return stringContent.StartsWith("\"", StringComparison.InvariantCultureIgnoreCase) &&
                stringContent.EndsWith("\"", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}