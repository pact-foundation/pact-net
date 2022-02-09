using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReadMe.Consumer
{
  public class SomethingApiClient
  {
    private readonly HttpClient _client;

    public SomethingApiClient(Uri baseUri = null)
    {
      _client = new HttpClient { BaseAddress = baseUri ?? new Uri("http://my-api") };
    }

    public async Task<Something> GetSomething(string id)
    {
      string reasonPhrase;

      var request = new HttpRequestMessage(HttpMethod.Get, "/somethings/" + id);
      request.Headers.Add("Accept", "application/json");

      var response = await _client.SendAsync(request);

      var content = await response.Content.ReadAsStringAsync();
      var status = response.StatusCode;

      reasonPhrase = response.ReasonPhrase; //NOTE: any Pact mock provider errors will be returned here and in the response body

      request.Dispose();
      response.Dispose();

      if (status == HttpStatusCode.OK)
      {
        return !String.IsNullOrEmpty(content) ?
          JsonConvert.DeserializeObject<Something>(content)
          : null;
      }

      throw new Exception(reasonPhrase);
    }
  }
}
