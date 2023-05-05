<span align="center">

![logo](https://user-images.githubusercontent.com/53900/121775784-0191d200-cbcd-11eb-83dd-adc001b94519.png)

# Pact Net

[![Build status](https://github.com/pact-foundation/pact-net/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/pact-foundation/pact-net/tree/master)

#### Fast, easy and reliable testing for your APIs and microservices.

</span>

**Pact** is the de-facto API contract testing tool. Replace expensive and brittle end-to-end integration tests with fast, reliable and easy to debug unit tests.

- ⚡ Lightning fast
- 🎈 Effortless full-stack integration testing - from the front-end to the back-end
- 🔌 Supports HTTP/REST and event-driven systems
- 🛠️ Configurable mock server
- 😌 Powerful matching rules prevents brittle tests
- 🤝 Integrates with Pact Broker / PactFlow for powerful CI/CD workflows
- 🔡 Supports 12+ languages

**Why use Pact?**

Contract testing with Pact lets you:

- ⚡ Test locally
- 🚀 Deploy faster
- ⬇️ Reduce the lead time for change
- 💰 Reduce the cost of API integration testing
- 💥 Prevent breaking changes
- 🔎 Understand your system usage
- 📃 Document your APIs for free
- 🗄 Remove the need for complex data fixtures
- 🤷‍♂️ Reduce the reliance on complex test environments

Watch our [series](https://www.youtube.com/playlist?list=PLwy9Bnco-IpfZ72VQ7hce8GicVZs7nm0i) on the problems with end-to-end integrated tests, and how contract testing can help.

![----------](https://raw.githubusercontent.com/pactumjs/pactum/master/assets/rainbow.png)

## Documentation

### Tutorial (60 minutes)

[Learn everything in Pact Net in 60 minutes](https://github.com/DiUS/pact-workshop-dotnet-core-v3/)

### Upgrading from PactNet v3.x or earlier to v4.x

[Upgrade Guide](docs/upgrading-to-4.md)

Looking for PactNet v3.x? See the [`release/3.x` branch](https://github.com/pact-foundation/pact-net/tree/release/3.x).

## Need Help

- Join our community [slack workspace](http://pact-foundation.slack.com).
- Stack Overflow: https://stackoverflow.com/questions/tagged/pact
- Say 👋 on [Twitter @pact_up](https://twitter.com/pact_up)

## Installation

[Via Nuget](https://www.nuget.org/packages/PactNet/)

![----------](https://raw.githubusercontent.com/pactumjs/pactum/master/assets/rainbow.png)

## Usage

In the sections below, we provide a brief sample of the typical flow for Pact testing, written in the XUnit framework. To see the complete example and run it, check out the `Samples/ReadMe` folder.

### Writing a Consumer test

Pact is a consumer-driven contract testing tool, which is a fancy way of saying that the API `Consumer` writes a test to set out its assumptions and needs of its API `Provider`(s). By unit testing our API client with Pact, it will produce a `contract` that we can share to our `Provider` to confirm these assumptions and prevent breaking changes.

In this example, we are going to be testing our User API client, responsible for communicating with the `UserAPI` over HTTP. It currently has a single method `GetUser(id)` that will return a `User`.

Pact tests have a few key properties. We'll demonstrate a common example using the 3A `Arrange/Act/Assert` pattern.

```csharp
public class SomethingApiConsumerTests
{
    private readonly IPactBuilderV4 pactBuilder;

    public SomethingApiConsumerTests()
    {
        // Use default pact directory ..\..\pacts and default log
        // directory ..\..\logs
        var pact = Pact.V4("Something API Consumer", "Something API", new PactConfig());

        // or specify custom log and pact directories
        pact = Pact.V4("Something API Consumer", "Something API", new PactConfig
        {
            PactDir = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName}{Path.DirectorySeparatorChar}pacts"
        });

        // Initialize Rust backend
        this.pactBuilder = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
    {
        // Arrange
        this.pactBuilder
            .UponReceiving("A GET request to retrieve the something")
                .Given("There is a something with id 'tester'")
                .WithRequest(HttpMethod.Get, "/somethings/tester")
                .WithHeader("Accept", "application/json")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new
                {
                    id = "tester",
                    firstName = "Totally",
                    lastName = "Awesome"
                });

        await this.pactBuilder.VerifyAsync(async ctx =>
        {
            // Act
            var client = new SomethingApiClient(ctx.MockServerUri);
            var something = await client.GetSomething("tester");

            // Assert
            Assert.Equal("tester", something.Id);
        });
    }
}
```

![----------](https://raw.githubusercontent.com/pactumjs/pactum/master/assets/rainbow.png)

### Verifying a Provider

A provider test takes one or more pact files (contracts) as input, and Pact verifies that your provider adheres to the contract. In the simplest case, you can verify a provider as per below. In `SomethingApiFixture`, the provider is started. In `SomethingApiTests`, the fixture is verified against the pact files.

```csharp
public class SomethingApiFixture : IDisposable
{
    private readonly IHost server;
    public Uri ServerUri { get; }

    public SomethingApiFixture()
    {
        ServerUri = new Uri("http://localhost:9223");
        server = Host.CreateDefaultBuilder()
                     .ConfigureWebHostDefaults(webBuilder =>
                     {
                         webBuilder.UseUrls(ServerUri.ToString());
                         webBuilder.UseStartup<TestStartup>();
                     })
                     .Build();
        server.Start();
    }

    public void Dispose()
    {
        server.Dispose();
    }
}

public class SomethingApiTests : IClassFixture<SomethingApiFixture>
{
    private readonly SomethingApiFixture fixture;
    private readonly ITestOutputHelper output;

    public SomethingApiTests(SomethingApiFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;
    }

    [Fact]
    public void EnsureSomethingApiHonoursPactWithConsumer()
    {
        // Arrange
        var config = new PactVerifierConfig
        {
            Outputters = new List<IOutput>
            {
                // NOTE: PactNet defaults to a ConsoleOutput, however
                // xUnit 2 does not capture the console output, so this
                // sample creates a custom xUnit outputter. You will
                // have to do the same in xUnit projects.
                new XUnitOutput(output),
            },
        };

        string pactPath = Path.Combine("..",
                                       "..",
                                       "..",
                                       "..",
                                       "pacts",
                                       "Something API Consumer-Something API.json");

        // Act / Assert
        IPactVerifier pactVerifier = new PactVerifier("Something API", config);
        pactVerifier
            .WithHttpEndpoint(fixture.ServerUri)
            .WithFileSource(new FileInfo(pactPath))
            .WithProviderStateUrl(new Uri(fixture.ServerUri, "/provider-states"))
            .Verify();
    }
}
```

**IMPORTANT**: You can't use the [`Microsoft.AspNetCore.Mvc.Testing`](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing)
library to host your API for provider tests. If your tests are using `TestServer` or `WebApplicationFactory` then these are running
the API with a special in-memory test server instead of running on a real TCP socket. This means the Rust internals can't call the
API and therefore all of your provider tests will fail. You **must** host the API on a proper TCP socket, e.g. by using the `Host`
method shown in the sample above, so that they can be called from non-.Net code.

![----------](https://raw.githubusercontent.com/pactumjs/pactum/master/assets/rainbow.png)

### Messaging Pacts

For writing messaging pacts instead of requests/response pacts, see the [messaging pacts guide](docs/messaging-pacts.md).

![----------](https://raw.githubusercontent.com/pactumjs/pactum/master/assets/rainbow.png)

## Compatibility

### Operating System

Due to using a shared native library instead of C# for the main Pact logic only certain OSs are supported:

| OS           | Arch        | Support                                                            |
| ------------ | ----------- | -------------------------------------------------------------------|
| Windows      | x86         | ❌ No                                                              |
| Windows      | x64         | ✔️ Yes                                                             |
| Linux (libc) | ARM         | ❌ No                                                              |
| Linux (libc) | x86         | ❌ No                                                              |
| Linux (libc) | x64         | ✔️ Yes                                                             |
| Linux (musl) | Any         | ❌ [No](https://github.com/pact-foundation/pact-net/issues/374)    |
| OSX          | x64         | ✔️ Yes                                                             |
| OSX          | ARM (M1/M2) | ⚠️ [Alpha](https://github.com/pact-foundation/pact-net/issues/451) |

### Pact Specification

| Version | Status     | [Spec] Compatibility | Install            |
| ------- | ---------- | -------------------- | ------------------ |
| 5.x     | Beta       | 2, 3, 4              | See [installation] |
| 4.x     | Stable     | 2, 3                 | See [installation] |
| 3.x     | Deprecated | 2                    |                    |

## Roadmap

The [roadmap](https://docs.pact.io/roadmap/) for Pact and Pact Net is outlined on our main website.

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md).

[spec]: https://github.com/pact-foundation/pact-specification
[pact wiki]: https://github.com/pact-foundation/pact-ruby/wiki
[getting started with pact]: http://dius.com.au/2016/02/03/microservices-pact/
[pact website]: http://docs.pact.io/
[libraries]: https://github.com/pact-foundation/pact-reference/releases
[cli tools]: https://github.com/pact-foundation/pact-reference/releases
[installation]: #installation
[message support]: https://github.com/pact-foundation/pact-specification/tree/version-3#introduces-messages-for-services-that-communicate-via-event-streams-and-message-queues
[pact broker]: https://github.com/pact-foundation/pact_broker
[hosted broker]: pact.dius.com.au
[can-i-deploy tool]: https://github.com/pact-foundation/pact_broker/wiki/Provider-verification-results
[pactflow]: https://pactflow.io
