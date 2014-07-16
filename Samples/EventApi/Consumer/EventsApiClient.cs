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
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);

            var request = new HttpRequestMessage(HttpMethod.Get, "/stats/status");

            var response = client.SendAsync(request);

            var content = response.Result.Content.ReadAsStringAsync().Result;
            var status = response.Result.StatusCode;

            if (status == HttpStatusCode.OK && content.Equals("alive"))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);
            
            var request = new HttpRequestMessage(HttpMethod.Get, "/events");
            request.Headers.Add("Accept", "application/json");

            var response = client.SendAsync(request);

            var content = response.Result.Content.ReadAsStringAsync().Result;
            var status = response.Result.StatusCode;

            if (status == HttpStatusCode.OK)
            {
                return !String.IsNullOrEmpty(content) ?
                    JsonConvert.DeserializeObject<IEnumerable<Event>>(content, _jsonSettings)
                    : new List<Event>();
            }

            throw new Exception("Server responded with a non 200 status code");
        }

        public Event GetEventById(Guid id)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);

            var request = new HttpRequestMessage(HttpMethod.Get, String.Format("/events/{0}", id));
            request.Headers.Add("Accept", "application/json");

            var response = client.SendAsync(request);

            var content = response.Result.Content.ReadAsStringAsync().Result;
            var status = response.Result.StatusCode;

            if (status == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<Event>(content, _jsonSettings);
            }

            throw new ArgumentException(String.Format("Event with id '{0}' could not be found", id));
        }

        public IEnumerable<Event> GetEventsByType(string eventType)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);

            var request = new HttpRequestMessage(HttpMethod.Get, String.Format("/events?type={0}", eventType));
            request.Headers.Add("Accept", "application/json");

            var response = client.SendAsync(request);

            var content = response.Result.Content.ReadAsStringAsync().Result;
            var status = response.Result.StatusCode;

            if (status == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<IEnumerable<Event>>(content, _jsonSettings);
            }

            throw new ArgumentException(String.Format("Event with type '{0}' could not be found", eventType));
        }

        public void CreateEvent(Guid eventId, string eventType = "DetailsView")
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);

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

            if (result.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception("Server responded with a non 201 status code");
            }
        }
    }
}
