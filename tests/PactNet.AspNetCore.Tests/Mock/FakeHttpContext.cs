using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using AuthenticationManager = Microsoft.AspNetCore.Http.Authentication.AuthenticationManager;

namespace PactNet.AspNetCore.Tests.Mock
{
    public class FakeHttpContext : HttpContext
    {
        public FakeHttpRequest _request;
        private FakeHttpResponse _response;

        public FakeHttpContext(HttpMethod requestMethod, string requestUri, dynamic requestBody, HttpStatusCode responseStatusCodes)
        {
            _request = new FakeHttpRequest()
            {
                Method = requestMethod.ToString(),
                Path = new PathString(requestUri),
                Body = requestBody != null ? GenerateStreamFromString(JsonSerializer.Serialize(requestBody)) : null
            };
            _response = new FakeHttpResponse();
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public override IFeatureCollection Features { get; }
        public override HttpRequest Request => _request;
        public override HttpResponse Response => _response;
        public override ConnectionInfo Connection { get; }
        public override WebSocketManager WebSockets { get; }
        public override AuthenticationManager Authentication { get; }
        public override ClaimsPrincipal User { get; set; }
        public override IDictionary<object, object> Items { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override CancellationToken RequestAborted { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ISession Session { get; set; }

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
