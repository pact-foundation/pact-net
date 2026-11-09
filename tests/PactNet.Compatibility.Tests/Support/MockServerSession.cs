using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using PactNet.Exceptions;
using PactNet.Infrastructure.Outputters;
using PactNet.Interop;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// A PactNet consumer test held open across Gherkin steps: the mock server starts inside
    /// <see cref="IPactBuilderV2.VerifyAsync"/> and the interact callback only completes once "the pact test is done"
    /// </summary>
    internal sealed class MockServerSession : IAsyncDisposable
    {
        public const string Consumer = "compatibility-suite-c";
        public const string Provider = "p";

        private readonly TaskCompletionSource<Uri> started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource finished = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly CapturingOutput output = new();
        private Task verifyTask;

        private MockServerSession(string pactDir)
        {
            this.PactDir = pactDir;
        }

        public string PactDir { get; }

        public string PactFile => Path.Combine(this.PactDir, $"{Consumer}-{Provider}.json");

        public Uri BaseUri { get; private set; }

        public bool IsDone { get; private set; }

        /// <summary>
        /// Result of verification once done: true when the mock server reported no mismatches
        /// </summary>
        public bool Ok { get; private set; }

        public Exception VerifyError { get; private set; }

        /// <summary>
        /// Mismatches captured at the moment the test was marked done (the mock server is gone afterwards)
        /// </summary>
        public JsonArray FinalMismatches { get; private set; }

        public string Output => this.output.ToString();

        public static async Task<MockServerSession> StartAsync(IReadOnlyList<InteractionDefinition> interactions)
        {
            string pactDir = Path.Combine(Path.GetTempPath(), "pactnet-compat", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pactDir);

            var session = new MockServerSession(pactDir);
            var config = new PactConfig
            {
                PactDir = pactDir,
                Outputters = new IOutput[] { session.output },
                LogLevel = PactLogLevel.Information
            };

            IPactBuilderV2 builder;
            CapturingHttpPactDriver capturing = null;

            if (interactions.Any(i => i.Body?.IsBinary == true || i.ResponseBody?.IsBinary == true))
            {
                // Binary bodies need the interaction handle, which the public DSL never exposes
                PactHandle pact = NativeInterop.NewPact(Consumer, Provider);
                NativeInterop.WithSpecification(pact, PactSpecification.V2);
                capturing = new CapturingHttpPactDriver(pact);
                builder = new PactBuilder(capturing, config);
            }
            else
            {
                builder = Pact.V2(Consumer, Provider, config).WithHttpInteractions();
            }

            foreach (InteractionDefinition interaction in interactions)
            {
                Define(builder, capturing, interaction);
            }

            session.verifyTask = Task.Run(() => builder.VerifyAsync(async ctx =>
            {
                session.started.TrySetResult(ctx.MockServerUri);
                await session.finished.Task;
            }));

            Task first = await Task.WhenAny(session.started.Task, session.verifyTask);
            if (first != session.started.Task)
            {
                await session.verifyTask; // surfaces the start-up exception
                throw new InvalidOperationException("Mock server exited before it started");
            }

            session.BaseUri = await session.started.Task;
            return session;
        }

        private static void Define(IPactBuilderV2 builder, CapturingHttpPactDriver capturing, InteractionDefinition interaction)
        {
            IRequestBuilderV2 request = builder.UponReceiving($"Interaction {interaction.No}")
                                               .WithRequest(interaction.Method, interaction.Path);

            foreach (KeyValuePair<string, string> query in InteractionDefinition.ParseQuery(interaction.Query))
            {
                request = request.WithQuery(query.Key, query.Value);
            }

            foreach (KeyValuePair<string, string> header in interaction.Headers)
            {
                request = request.WithHeader(header.Key, header.Value);
            }

            if (interaction.Body is { } body)
            {
                if (body.IsBinary)
                {
                    AddBinaryBody(capturing, InteractionPart.Request, body);
                }
                else
                {
                    request = request.WithBody(body.Text, body.ContentType);
                }
            }

            IResponseBuilderV2 response = request.WillRespond().WithStatus(interaction.Status);

            foreach (KeyValuePair<string, string> header in interaction.ResponseHeaders)
            {
                response = response.WithHeader(header.Key, header.Value);
            }

            if (interaction.ResponseBody is { } responseBody)
            {
                if (responseBody.IsBinary)
                {
                    AddBinaryBody(capturing, InteractionPart.Response, responseBody);
                }
                else
                {
                    response.WithBody(responseBody.Text, interaction.ResponseContentType ?? responseBody.ContentType);
                }
            }
            else if (interaction.ResponseContentType != null)
            {
                response.WithHeader("Content-Type", interaction.ResponseContentType);
            }
        }

        private static void AddBinaryBody(CapturingHttpPactDriver capturing, InteractionPart part, BodyDefinition body)
        {
            if (!CompatFfi.WithBinaryBody(capturing.LastInteraction, part, body.ContentType, body.Bytes, (UIntPtr)body.Bytes.Length))
            {
                throw new InvalidOperationException($"pactffi_with_binary_body failed for the {part} body");
            }
        }

        /// <summary>
        /// Current mismatches straight from the running mock server (the DSL only exposes them as an exception message)
        /// </summary>
        public JsonArray LiveMismatches()
        {
            if (this.IsDone)
            {
                return this.FinalMismatches;
            }

            IntPtr ptr = NativeInterop.MockServerMismatches(this.BaseUri.Port);
            string json = ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
            return string.IsNullOrWhiteSpace(json) ? new JsonArray() : (JsonArray)JsonNode.Parse(json);
        }

        public async Task<HttpResponse> SendAsync(InteractionDefinition request)
        {
            using var client = new HttpClient { BaseAddress = this.BaseUri };
            string target = request.Path + (string.IsNullOrEmpty(request.Query) ? string.Empty : "?" + request.Query);
            using var message = new HttpRequestMessage(new HttpMethod(request.Method), target);

            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                {
                    message.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (request.Body is { } body)
            {
                message.Content = new ByteArrayContent(body.Bytes);
                string contentType = request.Headers.FirstOrDefault(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)).Value ?? body.ContentType;
                message.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);
            }

            using HttpResponseMessage response = await client.SendAsync(message);
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();

            return new HttpResponse((int)response.StatusCode, response.Content.Headers.ContentType?.ToString(), bytes);
        }

        /// <summary>
        /// Let VerifyAsync complete: it checks mismatches and writes the pact file only if there are none
        /// </summary>
        public async Task FinishAsync()
        {
            if (this.IsDone)
            {
                return;
            }

            this.FinalMismatches = this.LiveMismatches();
            this.IsDone = true;
            this.finished.TrySetResult();

            try
            {
                await this.verifyTask;
                this.Ok = true;
            }
            catch (PactFailureException e)
            {
                this.Ok = false;
                this.VerifyError = e;
            }
        }

        public async ValueTask DisposeAsync()
        {
            this.finished.TrySetResult();

            try
            {
                if (this.verifyTask != null)
                {
                    await this.verifyTask;
                }
            }
            catch (Exception)
            {
                // the scenario's outcome has already been decided by its steps
            }

            try
            {
                Directory.Delete(this.PactDir, true);
            }
            catch (IOException)
            {
            }
        }

        private sealed class CapturingOutput : IOutput
        {
            private readonly StringBuilder buffer = new();

            public void WriteLine(string line)
            {
                lock (this.buffer)
                {
                    this.buffer.AppendLine(line);
                }
            }

            public override string ToString()
            {
                lock (this.buffer)
                {
                    return this.buffer.ToString();
                }
            }
        }
    }

    internal sealed record HttpResponse(int Status, string ContentType, byte[] Body);
}
