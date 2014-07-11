using System;
using System.Text;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class HttpBodyContent
    {
        private readonly string _defaultContentType = "text/plain";
        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        public string Content { get; set; }

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
                    _contentType = _defaultContentType;
                }
                return _contentType;
            }
            set { _contentType = value; }
        }

        private Encoding _encoding;
        public Encoding Encoding
        {
            get { return _encoding ?? (_encoding = _defaultEncoding); }
            set { _encoding = value; }
        }
    }
}