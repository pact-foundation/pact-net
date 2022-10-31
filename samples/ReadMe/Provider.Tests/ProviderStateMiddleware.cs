using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ReadMe.Provider.Tests
{
    public class ProviderStateMiddleware
    {
        private readonly IDictionary<string, (Action Setup, Action Teardown)> providerStates;
        private readonly RequestDelegate next;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            this.next = next;
            providerStates = new Dictionary<string, (Action, Action)>
            {
                {
                    "There is a something with id 'tester'",
                    (AddTesterIfItDoesntExist, RemoveTester)
                }
            };
        }

        private void AddTesterIfItDoesntExist()
        {
            var dataDirectory = Directory.CreateDirectory(Path.Combine("..", "..", "..", "data"));
            var dataFilePath = Path.Combine(dataDirectory.FullName, "somethings.json");
            var fileData = File.Exists(dataFilePath) ? File.ReadAllText(dataFilePath) : null;
            var somethingsData = string.IsNullOrEmpty(fileData)
                ? new List<Something>()
                : JsonConvert.DeserializeObject<List<Something>>(fileData);
            if (!somethingsData.Any(something => something.Id == "tester"))
            {
                somethingsData.Add(new Something()
                {
                    Id = "tester",
                    FirstName = "Totally",
                    LastName = "Awesome",
                });
            }
            File.WriteAllText(dataFilePath, JsonConvert.SerializeObject(somethingsData));
        }

        private void RemoveTester()
        {
            Directory.Delete(Path.Combine("..", "..", "..", "data"), true);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path
                .Value?.StartsWith("/provider-states") ?? false)
            {
                await next.Invoke(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method == HttpMethod.Post.ToString()
                && context.Request.Body != null)
            {
                string jsonRequestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (!string.IsNullOrEmpty(providerState?.State))
                {
                    var (setupAction, teardownAction) = providerStates[providerState.State];
                    var action = providerState.Action is ProviderStateAction.Setup ? setupAction : teardownAction;
                    action.Invoke();
                }

                await context.Response.WriteAsync(string.Empty);
            }
        }
    }
}
