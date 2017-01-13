using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Consumer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Consumer
{
    public class EventsApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public EventsApiClient(string baseUri = null, string authToken = null)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri ?? "http://my.api/v2/capture") };

            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
            }
        }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public async Task<bool> IsAlive()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/stats/status");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw await GetResponseException(request, response);
                }
                var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync(), _jsonSettings);
                return responseContent.alive;
            }
            finally
            {
                Dispose(request, response);
            }
        }

        public async Task<DateTime?> UpSince()
        {
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, "/stats/status");
            statusRequest.Headers.Add("Accept", "application/json");

            var statusResponse = await _httpClient.SendAsync(statusRequest);

            try
            {
                StatusResponseBody statusResponseBody;

                if (statusResponse.StatusCode == HttpStatusCode.OK)
                {
                    statusResponseBody = JsonConvert.DeserializeObject<StatusResponseBody>(await statusResponse.Content.ReadAsStringAsync(), _jsonSettings);
                }
                else
                {
                    throw await GetResponseException(statusRequest, statusResponse);
                }

                if (statusResponseBody.Alive)
                {
                    var uptimeLink = statusResponseBody.Links.Single(x => x.Key.Equals("uptime")).Value.Href;

                    if (!string.IsNullOrEmpty(uptimeLink))
                    {
                        var uptimeRequest = new HttpRequestMessage(HttpMethod.Get, uptimeLink);
                        uptimeRequest.Headers.Add("Accept", "application/json");

                        var uptimeResponse = await _httpClient.SendAsync(uptimeRequest);
                        try
                        {
                            if (uptimeResponse.StatusCode != HttpStatusCode.OK)
                            {
                                throw await GetResponseException(uptimeRequest, uptimeResponse);
                            }
                            var uptimeResponseBody =
                                JsonConvert.DeserializeObject<UptimeResponseBody>(
                                    await uptimeResponse.Content.ReadAsStringAsync(), _jsonSettings);
                            return uptimeResponseBody.UpSince;
                        }
                        finally
                        {
                            Dispose(uptimeRequest, uptimeResponse);
                        }
                    }
                }
            }
            finally
            {
                Dispose(statusRequest, statusResponse);
            }

            return null;
        }

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/events");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw await GetResponseException(request, response);
                }
                var content = await response.Content.ReadAsStringAsync();
                return !string.IsNullOrEmpty(content)
                    ? JsonConvert.DeserializeObject<IEnumerable<Event>>(content, _jsonSettings)
                    : new List<Event>();

            }
            finally
            {
                Dispose(request, response);
            }
        }

        public async Task<Event> GetEventById(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/events/{id}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<Event>(await response.Content.ReadAsStringAsync(),
                        _jsonSettings);
                }
                throw await GetResponseException(request, response);
            }
            finally
            {
                Dispose(request, response);
            }
        }

        public async Task<IEnumerable<Event>> GetEventsByType(string eventType)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/events?type={eventType}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Event>>(
                        await response.Content.ReadAsStringAsync(), _jsonSettings);
                }
                throw await GetResponseException(request, response);
            }
            finally
            {
                Dispose(request, response);
            }
        }

        public async Task CreateEvent(Guid eventId, string eventType = "DetailsView")
        {
            var @event = new
            {
                EventId = eventId,
                Timestamp = DateTimeFactory.Now().ToString("O"),
                EventType = eventType
            };

            var eventJson = JsonConvert.SerializeObject(@event, _jsonSettings);
            var requestContent = new StringContent(eventJson, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/events") { Content = requestContent };

            var response = await _httpClient.SendAsync(request);

            try
            {
                var statusCode = response.StatusCode;
                if (statusCode != HttpStatusCode.Created)
                {
                    throw await GetResponseException(request, response);
                }
            }
            finally
            {
                Dispose(request, response);
            }
        }

        private static async Task<HttpRequestException> GetResponseException(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            return new HttpRequestException(
                $"The Events API request for {failedRequest.Method.ToString().ToUpperInvariant()} {failedRequest.RequestUri} failed. Response Status: {(int) failedResponse.StatusCode}, Response Body: {await failedResponse.Content.ReadAsStringAsync()}");
        }

        public void Dispose()
        {
            Dispose(_httpClient);
        }

        public void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables.Where(d => d != null))
            {
                disposable.Dispose();
            }
        }

        public async Task CreateBlob(Guid id, byte[] content, string fileName)
        {
            var bytes = new ByteArrayContent(content);
            bytes.Headers.ContentDisposition = new ContentDispositionHeaderValue("file") { FileName = fileName };
            bytes.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var request = new HttpRequestMessage(HttpMethod.Post, $"/blobs/{id}") { Content = bytes };

            var response = await _httpClient.SendAsync(request);

            try
            {
                var statusCode = response.StatusCode;
                if (statusCode != HttpStatusCode.Created)
                {
                    throw await GetResponseException(request, response);
                }
            }
            finally
            {
                Dispose(request, response);
            }
        }

        public async Task<byte[]> GetBlob(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/blobs/{id}");

            var response = await _httpClient.SendAsync(request);

            try
            {
                var statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.Created)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                throw await GetResponseException(request, response);
            }
            finally
            {
                Dispose(request, response);
            }
        }
    }
}
