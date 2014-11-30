using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Consumer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Consumer
{
    public class EventsApiClient
    {
        public string BaseUri { get; set; }

        public EventsApiClient(string baseUri = null)
        {
            BaseUri = baseUri ?? "http://my.api/v2/capture";
        }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public bool IsAlive()
        {
            using (var client = HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/stats/status");
                request.Headers.Add("Accept", "application/json");

                var response = client.SendAsync(request);

                var result = response.Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var status = result.StatusCode;

                if (status == HttpStatusCode.OK)
                {
                    var responseContent = JsonConvert.DeserializeObject<dynamic>(content, _jsonSettings);
                    return responseContent.alive;
                }

                request.Dispose();
                response.Dispose();
                result.Dispose();
            }

            return false;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            string reasonPhrase;
            using (var client = HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/events");
                request.Headers.Add("Accept", "application/json");

                var response = client.SendAsync(request);
                var result = response.Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var status = result.StatusCode;

                reasonPhrase = result.ReasonPhrase;

                if (status == HttpStatusCode.OK)
                {
                    return !String.IsNullOrEmpty(content)
                               ? JsonConvert.DeserializeObject<IEnumerable<Event>>(content, _jsonSettings)
                               : new List<Event>();
                }

                request.Dispose();
                response.Dispose();
                result.Dispose();
            }

            throw new InvalidOperationException(reasonPhrase);
        }

        public Event GetEventById(Guid id)
        {
            string reasonPhrase;
            using (var client = HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, String.Format("/events/{0}", id));
                request.Headers.Add("Accept", "application/json");

                var response = client.SendAsync(request);
                var result = response.Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var status = result.StatusCode;

                reasonPhrase = result.ReasonPhrase;

                if (status == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<Event>(content, _jsonSettings);
                }

                request.Dispose();
                response.Dispose();
                result.Dispose();
            }

            throw new InvalidOperationException(reasonPhrase);
        }

        public IEnumerable<Event> GetEventsByType(string eventType)
        {
            string reasonPhrase;
            using (var client = HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, String.Format("/events?type={0}", eventType));
                request.Headers.Add("Accept", "application/json");

                var response = client.SendAsync(request);
                var result = response.Result;
                var content = result.Content.ReadAsStringAsync().Result;
                var status = result.StatusCode;

                reasonPhrase = result.ReasonPhrase;

                if (status == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Event>>(content, _jsonSettings);
                }

                request.Dispose();
                response.Dispose();
                result.Dispose();
            }

            throw new InvalidOperationException(reasonPhrase);
        }

        public void CreateEvent(Guid eventId, string eventType = "DetailsView")
        {
            HttpStatusCode statusCode;
            string reasonPhrase;

            using (var client = HttpClient())
            {
                var @event = new
                {
                    EventId = eventId,
                    Timestamp = DateTimeFactory.Now().ToString("O"),
                    EventType = eventType
                };

                var eventJson = JsonConvert.SerializeObject(@event, _jsonSettings);

                var requestContent = new StringContent(eventJson, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, "/events");
                request.Content = requestContent;

                var response = client.SendAsync(request);
                var result = response.Result;
                statusCode = result.StatusCode;
                reasonPhrase = result.ReasonPhrase;

                request.Dispose();
                response.Dispose();
                result.Dispose();
            }

            if (statusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException(reasonPhrase);
            }
        }

        private HttpClient HttpClient()
        {
            return new HttpClient { BaseAddress = new Uri(BaseUri) };
        } 
    }
}
