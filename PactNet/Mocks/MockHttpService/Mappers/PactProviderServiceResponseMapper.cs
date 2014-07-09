using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public class PactProviderServiceResponseMapper : IPactProviderServiceResponseMapper
    {
        public PactProviderServiceResponse Convert(HttpResponseMessage from)
        {
            if (from == null)
            {
                return null;
            }

            var to = new PactProviderServiceResponse
            {
                Status = (int) from.StatusCode,
                Headers = ConvertHeaders(from.Headers, from.Content.Headers)
            };

            var responseContent = from.Content.ReadAsStringAsync().Result;
            if (!String.IsNullOrEmpty(responseContent))
            {
                to.Body = JsonConvert.DeserializeObject<dynamic>(responseContent);
            }

            return to;
        }

        //TODO: This can be split out into a seperate mapper
        private Dictionary<string, string> ConvertHeaders(HttpResponseHeaders responseHeaders, HttpContentHeaders contentHeaders)
        {
            if ((responseHeaders == null || !responseHeaders.Any()) &&
                (contentHeaders == null || !contentHeaders.Any()))
            {
                return null;
            }

            var headers = new Dictionary<string, string>();

            if (responseHeaders != null && responseHeaders.Any())
            {
                foreach (var responseHeader in responseHeaders)
                {
                    headers.Add(responseHeader.Key, responseHeader.Value.First());
                }
            }

            if (contentHeaders != null && contentHeaders.Any())
            {
                foreach (var contentHeader in contentHeaders)
                {
                    headers.Add(contentHeader.Key, contentHeader.Value.First());
                }
            }

            return headers;
        }
    }
}