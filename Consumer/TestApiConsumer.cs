using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Consumer
{
    public class TestApiConsumer
    {
        public string BaseUri { get; set; }

        public TestApiConsumer(string baseUri = null)
        {
            BaseUri = baseUri ?? "http://infra.api/v2/capture";
        }

        public IEnumerable<dynamic> GetAllEvents()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Get, "/events");

            var response = client.SendAsync(request);

            var content = response.Result.Content.ReadAsStringAsync().Result;
            var status = response.Result.StatusCode;

            if (status == HttpStatusCode.OK)
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                return !String.IsNullOrEmpty(content) ? 
                    JsonConvert.DeserializeObject<IEnumerable<dynamic>>(content, jsonSettings)
                    : new List<dynamic>();
            }

            throw new Exception("Server responded with a non 200 status code");
        }
    }
}
