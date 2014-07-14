using System;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class HttpVerbMapper : IHttpVerbMapper
    {
        private static readonly IDictionary<string, HttpVerb> Map = new Dictionary<string, HttpVerb>()
        {
            { "GET", HttpVerb.Get },
            { "POST", HttpVerb.Post },
            { "PUT", HttpVerb.Put },
            { "DELETE", HttpVerb.Delete },
            { "HEAD", HttpVerb.Head },
            { "PATCH", HttpVerb.Patch }
        };

        public HttpVerb Convert(string from)
        {
            if (!Map.ContainsKey(from))
            {
                throw new ArgumentException(String.Format("Cannot map {0} to a HttpVerb, no matching item has been registered.", from));
            }

            return Map[from];
        }
    }
}