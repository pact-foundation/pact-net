Upgrading to PactNet 4.x
========================

PactNet 4.0.0 is a major rewrite of PactNet to be based on the new [Rust core library]
instead of the older Ruby library. This has the following benefits:

- Upgrade to [Pact Specification v3]
- Increased performance for consumer and provider tests
- A new fluent API
- The mock server runs in-process so no more having to allocate a port and no rogue
  `ruby.exe` processes left running
- The Rust core library is a single file so no more file-path length problems on Windows
- A single NuGet to install (`PactNet`) instead of a NuGet per OS/arch

However, due to the Rust library working very differently to the Ruby library, this means
that there are a number of breaking changes. This guide highlights those changes so that
you can migrate your existing tests to v4.x and beyond.

Steps
-----

1. Uninstall any OS-specific NuGets you have installed (such as `PactNet.Windows`) so that
   only the `PactNet` NuGet is installed, at version 4.0.0 or greater.
2. Migrate your tests to the new API (see details below).

Consumer Tests
--------------

Consumer tests have the most impactful breaking changes due to the way the mock server now
runs in-process, and at the same time the API itself has been rewritten to make it more
extensible in future without causing further breaking changes. A typical consumer test in
v4.x is:

```csharp
public class ConsumerTests
{
    private readonly IPactBuilderV3 pact;

    public ConsumerTests(ITestOutputHelper output)
    {
        var config = new PactConfig
        {
            PactDir = "../../../pacts/",
            Outputters = new[]
            {
                new XUnitOutput(output)
            },
            DefaultJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        // you select which specification version you wish to use by calling either V2 or V3
        IPactV3 pact = Pact.V3("My Consumer", "My Provider", config);

        // the pact builder is created in the constructor so it's unique to each test
        this.pact = pact.UsingNativeBackend();
    }

    [Fact]
    public async Task GetAllEvents_WhenCalled_ReturnsAllEvents()
    {
        var example = new Event
        {
            EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
            Timestamp = DateTime.Parse("2014-06-30T01:38:00.8518952"),
            EventType = "SearchView"
        };

        // create the expectation(s) using the fluent API, first the request and then the response
        this.pact
            .UponReceiving("a request to retrieve all events")
                .WithRequest(HttpMethod.Get, "/events")
                .WithHeader("Accept", "application/json")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(Match.MinType(new
                {
                    eventId = Match.Type("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    timestamp = Match.Type("2014-06-30T01:38:00.8518952"),
                    eventType = Match.Regex("SearchView", "SearchView|DetailsView")
                }, 1));

        await this.pact.VerifyAsync(async ctx =>
        {
            // all API calls must happen inside this lambda, using the URL provided by the context argument
            var client = new EventsApiClient(ctx.MockServerUri);

            IEnumerable<Event> events = await client.GetAllEvents();

            events.Should().BeEquivalentTo(new[] { example });
        });

        // the mock server is no longer running once VerifyAsync returns
    }
}
```

There are some very noticeable differences to PactNet v3.x and below:

- Each test runs independently of all the others, whereas in PactNet v3.x the mock server must
  be started once with the use of `IClassFixture` or a collection fixture. This also means there's
  no need to call `ClearInteractions()` between each consumer test as the mock server is not shared
  between them.
- The mock server doesn't need to be given a unique port - a free port is automatically assigned.
  You can optionally provide a port if you want to, but this isn't necessary.
- All API calls must happen inside `VerifyAsync` - the mock server is started when the call to
  `VerifyAsync` is made and shut down when it returns. The lambda you provide is executed whilst
  the server is active and the context argument provided to the lambda supplies information such
  as the mock server URL.
- The consumer expectations themselves are defined using a fluent API in which the request is
  defined, followed by the response. The API is more verbose but allows for additional features
  such as multiple values for request headers.
- Additional options are available for `Match`, such as specific numeric matchers like `Match.Decimal`.
- Pact files are always written in merge mode. This means if you run a single interaction test
  then the result will be merged into any existing file. It's important that CI runs delete any
  pact files prior to executing any consumer tests otherwise they may contain stale interactions.

Provider Tests
--------------

Provider tests are more similar to the API from v3.x. A typical provider test using the Pact Broker
as a source would be:

```csharp
// A test fixture ensures the API is started once and stopped at the end of the test run
public class ProviderFixture : IDisposable
{
    private readonly IHost server;

    public Uri ServerUri { get; }

    public ProviderFixture()
    {
        this.ServerUri = new Uri("http://localhost:9222");

        this.server = Host.CreateDefaultBuilder()
                          .ConfigureWebHostDefaults(webBuilder =>
                          {
                              webBuilder.UseUrls(this.ServerUri.ToString());
                              webBuilder.UseStartup<TestStartup>();
                          })
                          .Build();

        this.server.Start();
    }

    public void Dispose()
    {
        this.server.Dispose();
    }
}

public class EventApiTests : IClassFixture<ProviderFixture>
{
    private readonly ProviderFixture fixture;
    private readonly ITestOutputHelper output;

    public EventApiTests(ProviderFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;
    }

    [Fact]
    public void VerifyLatestPacts()
    {
        string version = Environment.GetEnvironmentVariable("VERSION");
        string branch = Environment.GetEnvironmentVariable("BRANCH");
        string buildUri = Environment.GetEnvironmentVariable("BUILD_URL");

        var config = new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Information,
            Outputters = new List<IOutput>
            {
                new XUnitOutput(this.output)
            }
        };

        IPactVerifier verifier = new PactVerifier(config);

        verifier.ServiceProvider("My Provider", this.fixture.ServerUri)
                .WithPactBrokerSource(new Uri("https://broker.example.org"), options =>
                {
                    options.ConsumerVersionSelectors(new ConsumerVersionSelector { MainBranch = true, Latest = true })
                           .PublishResults(version, results =>
                           {
                               results.ProviderBranch(branch)
                                      .BuildUri(new Uri(buildUri));
                           });
                })
                .WithProviderStateUrl(new Uri(this.fixture.ServerUri, "/provider-states"))
                .Verify();
    }
}
```

This example would retrieve all pacts from the broker which match the consumer version selectors. In this case
this is verifying the latest version from the main branch of each consumer, but you can provide multiple selectors.
When the verification is complete the results will be published back to the broker with the version, branch and
build URL provided.

There are additional options for the source (such as a single file, a directory or a URL) and additional
options for the Pact Broker, such as providing authentication credentials. You can explore the fluent API
to see what options are available.

Other Changes
-------------

- The `PactPublisher` class is deprecated in favour of using the [Pact Broker CLI tool] to publish pact files
  to the broker.
- Custom headers during provider verification aren't (currently) supported.
  - It is recommended that verification tests run without authentication, but if authentication is absolutely
    required then see the samples for an example of how to intercept and override the `Authorization` header with
    middleware.

[Rust core library]: https://github.com/pact-foundation/pact-reference
[Pact Broker CLI tool]: https://docs.pact.io/pact_broker/client_cli
[Pact Specification v3]: https://github.com/pact-foundation/pact-specification/tree/version-3
