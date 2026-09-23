Plugin Interactions
===================

Pact [plugins](https://docs.pact.io/plugins) extend Pact to protocols and content types beyond HTTP and
plain messaging, for example gRPC/protobuf or CSV. PactNet exposes a "raw" plugin API which works with any
plugin, so that a new plugin can be used before a friendlier wrapper library exists for it.

Currently only **synchronous** plugin interactions are supported: a single request with one or more
responses, such as a gRPC unary call. Asynchronous (message-style) plugin interactions are not yet exposed.

Prerequisites
-------------

The plugin itself must be installed on the machine running the tests, on both the consumer and provider side.
Plugins live under `~/.pact/plugins/<name>-<version>` and are installed with the
[plugin CLI](https://docs.pact.io/implementation_guides/pact_plugins/cli), for example:

```shell
pact-plugin-cli install https://github.com/pact-foundation/pact-protobuf-plugin/releases/tag/v-0.4.0
```

Sample
------

See the [gRPC sample](../samples/Grpc/) for a working consumer and provider using the protobuf plugin.

Consumer Tests
--------------

A plugin pact is created from a V4 pact by naming the plugin, its version and the transport the mock
server should use. Each interaction is then defined by handing the plugin a content type it understands
plus a dictionary describing the interaction. The plugin, not PactNet, decides what that dictionary must
contain and how it is split into a request and its responses.

```csharp
public class GreeterClientTests : IDisposable
{
    private readonly ISynchronousPluginPactBuilderV4 pact;

    public GreeterClientTests(ITestOutputHelper output)
    {
        IPactV4 v4 = Pact.V4("Greeter Client", "Greeter Service", new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new[] { new XunitOutput(output) }
        });

        this.pact = v4.WithSynchronousPluginInteractions("protobuf", "0.4.0", transport: "grpc");
    }

    [Fact]
    public async Task SayHello_ReturnsGreeting()
    {
        var content = new Dictionary<string, object>
        {
            ["pact:proto"] = Path.GetFullPath("../../../Protos/greet.proto"),
            ["pact:proto-service"] = "Greeter/SayHello",
            ["pact:content-type"] = "application/protobuf",
            ["request"] = new { name = "matching(equalTo, 'foo')" },
            ["response"] = new { message = "matching(equalTo, 'Hello foo')" }
        };

        this.pact
            .UponReceiving("a request to say hello")
                .Given("the greeter is available")
                .WithContent("application/grpc", content);

        await this.pact.VerifyAsync(async ctx =>
        {
            var client = new GreeterClient(ctx.MockServerUri);

            string greeting = await client.SayHello("foo");

            greeting.Should().Be("Hello foo");
        });
    }

    public void Dispose() => this.pact.Dispose();
}
```

`Verify`/`VerifyAsync` starts a mock server using the requested transport, runs your client against it, checks
every interaction was matched, writes the pact file and prints the mock server logs, exactly as for HTTP pacts.
Dispose the builder when the test finishes so the underlying plugin resources are released.

### Building an interaction

Each call to `UponReceiving` starts a new interaction and a pact may contain as many as you need. The builder
has two kinds of call:

- `Given(...)` adds provider states and may be repeated.
- `WithContent(contentType, content)` sets the plugin contents. **It completes the interaction and must be the
  last call.** It returns nothing, so it cannot be chained further.

`WithContent` is deliberately single-shot. The plugin splits one contents dictionary into the request and its
responses. If contents were set a second time on the same interaction the request would be replaced but the
response would be appended, leaving the interaction with two responses, so PactNet does not allow it. Define a
second `UponReceiving` instead.

### Multiple interactions on one method

The gRPC mock server tracks whether each service method was called, not each interaction. If two interactions
target the same `Service/Method` and your client only exercises one of them, verification still passes. Give
interactions on the same method distinguishable requests and make sure your test calls each of them.

Provider Tests
--------------

Provider verification uses the normal `PactVerifier`. Point it at the running gRPC server as the HTTP endpoint;
the transport recorded in the pact file tells the verifier to hand the interaction to the plugin.

```csharp
this.verifier
    .WithHttpEndpoint(new Uri("http://localhost:5000"))
    .WithFileSource(new FileInfo(pactPath))
    .Verify();
```

Writing a plugin wrapper
------------------------

A wrapper library gives a plugin a protocol-specific fluent API on top of the raw builders, without needing
any access to PactNet internals. Wrap `ISynchronousPluginPactBuilderV4`, have your own request/response builders
assemble the contents dictionary, and call `WithContent` once when the user has finished describing the
interaction. Delegate `Verify`, `VerifyAsync` and `Dispose` straight through. The
[`PactNet.Extensions.Grpc`](https://github.com/pact-foundation/pact-net/pull/548) package is the reference
implementation of this pattern.
