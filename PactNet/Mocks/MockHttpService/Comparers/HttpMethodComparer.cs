using System;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpMethodComparer : IHttpMethodComparer
    {
        private readonly string _messagePrefix;

        public HttpMethodComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Compare(HttpVerb method1, HttpVerb method2)
        {
            Console.WriteLine("{0} has method set to {1}", _messagePrefix, method1);
            if (!method1.Equals(method2))
            {
                throw new CompareFailedException(method1, method2);
            }
        }
    }
}