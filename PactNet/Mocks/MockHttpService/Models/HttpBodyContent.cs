using System;
using System.Text;
using Newtonsoft.Json;
using PactNet.Configuration.Json;

namespace PactNet.Mocks.MockHttpService.Models
{
    internal class HttpBodyContent
    {
        private const string DefaultContentType = "text/plain";
        private readonly Encoding _defaultEncoding = Encoding.UTF8;
        private readonly bool _contentIsBase64Encoded = false;

        public dynamic Body { get; private set; }
        public string Content { get; private set; }

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

        private string _contentType;
        public string ContentType
        {
            get
            {
                if (String.IsNullOrEmpty(_contentType))
                {
                    _contentType = DefaultContentType;
                }
                return _contentType;
            }
        }

        private Encoding _encoding;
        public Encoding Encoding
        {
            get { return _encoding ?? (_encoding = _defaultEncoding); }
        }

        public HttpBodyContent(dynamic body, string contentType, Encoding encoding)
        {
            if (body == null)
            {
                throw new ArgumentException("body cannot be null");
            }

            _contentType = contentType;
            _encoding = encoding;

            if (IsJsonContentType())
            {
                string c = JsonConvert.SerializeObject(body, JsonConfig.ApiSerializerSettings);
                Content = c;
                Body = body;
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

        public HttpBodyContent(byte[] content, string contentType, Encoding encoding)
        {
            if (content == null)
            {
                throw new ArgumentException("content cannot be null");
            }

            _contentType = contentType;
            _encoding = encoding;

            if (IsJsonContentType())
            {
                var jsonContent = Encoding.GetString(content);
                Content = jsonContent;
                Body = JsonConvert.DeserializeObject<dynamic>(jsonContent);
            }
            else if (IsBinaryContentType())
            {
                Content = Encoding.GetString(content);
                Body = Convert.ToBase64String(content);
            }
            else
            {
                var stringContent = Encoding.GetString(content);
                Content = stringContent;
                Body = stringContent;
            }
        }

        private bool IsJsonContentType()
        {
            return ContentType.IndexOf("application/", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                ContentType.IndexOf("json", StringComparison.InvariantCultureIgnoreCase) > 0;
        }

        private bool IsBinaryContentType()
        {
            return ContentType.IndexOf("application/", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                ContentType.IndexOf("octet-stream", StringComparison.InvariantCultureIgnoreCase) > 0;
        }
    }
}