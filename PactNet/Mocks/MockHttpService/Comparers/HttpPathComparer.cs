using System;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpPathComparer : IHttpPathComparer
    {
        private readonly string _messagePrefix;

        public HttpPathComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Compare(string path1, string path2)
        {
            if (path1 == null)
            {
                return;
            }

            Console.WriteLine("{0} has path set to {1}", _messagePrefix, path1);

            if (!path1.Equals(path2))
            {
                throw new CompareFailedException(path1, path2);
            }
        }
    }
}