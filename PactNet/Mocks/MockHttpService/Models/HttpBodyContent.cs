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

        public dynamic Body { get; private set; }
        public string Content { get; private set; }

        public byte[] ContentBytes
        {
            get
            {
                return Content == null ? null : Encoding.GetBytes(Content);
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

            Body = body;
            Content = ConvertBodyToContent(body);
        }

        public HttpBodyContent(string content, string contentType, Encoding encoding)
        {
            if (content == null)
            {
                throw new ArgumentException("content cannot be null");
            }

            _contentType = contentType;
            _encoding = encoding;

            Content = content;
            Body = ConvertContentToBody(content);
        }

        private string ConvertBodyToContent(dynamic body)
        {
            if (ContentType.Equals("application/json"))
                return JsonConvert.SerializeObject(body, JsonConfig.ApiSerializerSettings);

            if (ContentType.Equals("application/octet-stream"))
                return Encoding.GetString(body);

            return body.ToString();
        }

        private dynamic ConvertContentToBody(string content)
        {
            if (ContentType.Equals("application/json")) 
                return JsonConvert.DeserializeObject<dynamic>(content);

            if (ContentType.Equals("application/octet-stream"))
                return Encoding.GetBytes(content);
            
            return content;
        }
    }
}