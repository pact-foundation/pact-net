using System;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpQueryStringComparer : IHttpQueryStringComparer
    {
        private readonly string _messagePrefix;

        public HttpQueryStringComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Compare(string query1, string query2)
        {
            if (String.IsNullOrEmpty(query1))
            {
                return;
            }

            Console.WriteLine("{0} has path set to {1}", _messagePrefix, query1);
            
            if (!query1.Equals(query2))
            {
                throw new CompareFailedException(query1, query2);
            }
        }
    }
}