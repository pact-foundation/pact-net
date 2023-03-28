using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly HttpClient httpClient;

        public EventsApiClient(Uri baseUri, string authToken = null)
        {
            this.httpClient = new HttpClient { BaseAddress = baseUri };

            if (!string.IsNullOrWhiteSpace(authToken))
            {
                this.httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
            }
        }

        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public async Task<bool> IsAlive()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/stats/status");
            request.Headers.Add("Accept", "application/json");

            var response = await this.httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync(), this.jsonSettings);
                    return responseContent.alive;
                }

                await RaiseResponseError(request, response);
            }
            finally
            {
                Dispose(request, response);
            }

            return false;
        }

        public async Task<DateTime?> UpSince()
        {
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, "/stats/status");
            statusRequest.Headers.Add("Accept", "application/json");

            var statusResponse = await this.httpClient.SendAsync(statusRequest);

            try
            {
                var statusResponseBody = new StatusResponseBody();

                if (statusResponse.StatusCode == HttpStatusCode.OK)
                {
                    statusResponseBody = JsonConvert.DeserializeObject<StatusResponseBody>(await statusResponse.Content.ReadAsStringAsync(), this.jsonSettings);
                }
                else
                {
                    await RaiseResponseError(statusRequest, statusResponse);
                }

                if (statusResponseBody.Alive)
                {
                    var uptimeLink = statusResponseBody.Links.Single(x => x.Key.Equals("uptime")).Value.Href;

                    if (!string.IsNullOrEmpty(uptimeLink))
                    {
                        var uptimeRequest = new HttpRequestMessage(HttpMethod.Get, uptimeLink);
                        uptimeRequest.Headers.Add("Accept", "application/json");

                        var uptimeResponse = await this.httpClient.SendAsync(uptimeRequest);
                        try
                        {
                            if (uptimeResponse.StatusCode == HttpStatusCode.OK)
                            {
                                var uptimeResponseBody = JsonConvert.DeserializeObject<UptimeResponseBody>(await uptimeResponse.Content.ReadAsStringAsync(), this.jsonSettings);
                                return uptimeResponseBody.UpSince;
                            }

                            await RaiseResponseError(uptimeRequest, uptimeResponse);
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

            var response = await this.httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return !string.IsNullOrEmpty(content)
                                ? JsonConvert.DeserializeObject<IEnumerable<Event>>(content, this.jsonSettings)
                                : new List<Event>();
                }

                await RaiseResponseError(request, response);
            }
            finally
            {
                Dispose(request, response);
            }

            return null;
        }

        public async Task<Event> GetEventById(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format("/events/{0}", id));
            request.Headers.Add("Accept", "application/json");

            var response = await this.httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<Event>(await response.Content.ReadAsStringAsync(), this.jsonSettings);
                }

                await RaiseResponseError(request, response);
            }
            finally
            {
                Dispose(request, response);
            }

            return null;
        }

        public async Task<IEnumerable<Event>> GetEventsByType(string eventType)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format("/events?type={0}", eventType));
            request.Headers.Add("Accept", "application/json");

            var response = await this.httpClient.SendAsync(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<IEnumerable<Event>>(await response.Content.ReadAsStringAsync(), this.jsonSettings);
                }

                await RaiseResponseError(request, response);
            }
            finally
            {
                Dispose(request, response);
            }

            return null;
        }

        public async Task CreateEvent(Guid eventId, string eventType = "DetailsView")
        {
            var @event = new
            {
                EventId = eventId,
                Timestamp = DateTimeFactory.Now().ToString("O"),
                EventType = eventType
            };

            var eventJson = JsonConvert.SerializeObject(@event, this.jsonSettings);
            var requestContent = new StringContent(eventJson, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/events") { Content = requestContent };

            var response = await this.httpClient.SendAsync(request);

            try
            {
                var statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.Created)
                {
                    return;
                }

                await RaiseResponseError(request, response);
            }
            finally
            {
                Dispose(request, response);
            }
        }
        public async Task<HttpStatusCode> UploadFile(FileInfo file)
        {

            using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

            var request = new MultipartFormDataContent();
            request.Headers.ContentType.MediaType = "multipart/form-data";

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            var fileName = file.Name;
            var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            var encodedFileName = Convert.ToBase64String(fileNameBytes);
            request.Add(fileContent, "file", fileName);
            request.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = fileName,
                FileNameStar = $"utf-8''{encodedFileName}"
            };

            HttpResponseMessage response = await this.httpClient.PostAsync("/events/upload-file", request);
            try
            {
                var statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.Created)
                {
                    return statusCode;
                }
                throw new HttpRequestException(
               string.Format("The Events API request for POST /upload-file failed. Response Status: {0}, Response Body: {1}",
               response.StatusCode,
               await response.Content.ReadAsStringAsync()));
            }
            finally
            {
                Dispose(request, response);
            }
        }
        private static async Task RaiseResponseError(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            throw new HttpRequestException(
                string.Format("The Events API request for {0} {1} failed. Response Status: {2}, Response Body: {3}",
                failedRequest.Method.ToString().ToUpperInvariant(),
                failedRequest.RequestUri,
                (int)failedResponse.StatusCode,
                await failedResponse.Content.ReadAsStringAsync()));
        }

        public void Dispose()
        {
            Dispose(this.httpClient);
        }

        public void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables.Where(d => d != null))
            {
                disposable.Dispose();
            }
        }
    }
}
