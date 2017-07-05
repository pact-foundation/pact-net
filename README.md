# PactNet
[![Build status](https://ci.appveyor.com/api/projects/status/5h4t9oerlhqcnwm8/branch/master?svg=true)](https://ci.appveyor.com/project/SEEKJobs/pact-net/branch/master)  
A .NET implementation of the Ruby consumer driven contract library, [Pact](https://github.com/realestate-com-au/pact).  
Pact is based off the specification found at https://github.com/pact-foundation/pact-specification.  

PactNet primarily provides a fluent .NET DSL for describing HTTP requests that will be made to a service provider and the HTTP responses the consumer expects back to function correctly.  
In documenting the consumer interactions, we can replay them on the provider and ensure the provider responds as expected. This basically gives us complete test symmetry and removes the basic need for integrated tests.  
PactNet also has the ability to support other mock providers should we see fit.

PactNet is Version 2.0 compliant, and we now use the [Ruby standalone engine](https://github.com/pact-foundation/pact-ruby-standalone) as we move towards a common core approach. To enable Version 2.0 support,  make sure you supply a `PactConfig` object with `SpecificationVersion = "2.0.0"` when creating the `PactBuilder`.  

In reaching Version 2.0 compliance, we have made some breaking changes. This readme details the current latest version.  
See [Version 1.0 readme](https://github.com/SEEK-Jobs/pact-net/blob/master/README_v1.md) for the previous version.  

From the [Pact Specification repo](https://github.com/pact-foundation/pact-specification)

> "Pact" is an implementation of "consumer driven contract" testing that allows mocking of responses in the consumer codebase, and verification of the interactions in the provider codebase. The initial implementation was written in Ruby for Rack apps, however a consumer and provider may be implemented in different programming languages, so the "mocking" and the "verifying" steps would be best supported by libraries in their respective project's native languages. Given that the pact file is written in JSON, it should be straightforward to implement a pact library in any language, however, to get the best experience and most reliability of out mixing pact libraries, the matching logic for the requests and responses needs to be identical. There is little confidence to be gained in having your pacts "pass" if the logic used to verify a "pass" is inconsistent between implementations.

Read more about Pact and the problems it solves at https://docs.pact.io/

Please feel free to contribute, we do accept pull requests.

## Usage
Below are some samples of usage.  
For examples of Version 2 usage, please see the [Samples](https://github.com/SEEK-Jobs/pact-net/blob/master/Samples/EventApi/Consumer.Tests/EventsApiConsumerTests.cs).  

We have also written some `//NOTE:` comments inline in the code to help explain what certain calls do.

**A few others things to note:**
1. When using `Match.Regex` you must supply a valid Ruby regular expression, as we currently use the Ruby core engine.
2. If you want to use SSL, you will need to ignore certification validation errors globally (currently we just generate a self signed cert). You can do this in the tests by adding `ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;` before any HTTP clients and the PactBuilder objects are instantiated. NOTE: DO NOT add this configuration to your production code!

### Installing

Via Nuget with [Install-Package PactNet](http://www.nuget.org/packages/PactNet)

### Service Consumer

#### 1. Build your client
Which may look something like this.

```c#
public class SomethingApiClient
{
  public string BaseUri { get; set; }

  public SomethingApiClient(string baseUri = null)
  {
    BaseUri = baseUri ?? "http://my-api";
  }

  public Something GetSomething(string id)
  {
    string reasonPhrase;

    using (var client = new HttpClient { BaseAddress = new Uri(BaseUri) })
    {
      var request = new HttpRequestMessage(HttpMethod.Get, "/somethings/" + id);
      request.Headers.Add("Accept", "application/json");

      var response = client.SendAsync(request);

      var content = response.Result.Content.ReadAsStringAsync().Result;
      var status = response.Result.StatusCode;

      reasonPhrase = response.Result.ReasonPhrase; //NOTE: any Pact mock provider errors will be returned here and in the response body

      request.Dispose();
      response.Dispose();

      if (status == HttpStatusCode.OK)
      {
        return !String.IsNullOrEmpty(content) ?
          JsonConvert.DeserializeObject<Something>(content)
          : null;
      }
    }

    throw new Exception(reasonPhrase);
  }
}
```

#### 2. Describe and configure the pact as a service consumer with a mock service
Create a new test case within your service consumer test project, using whatever test framework you like (in this case we used xUnit).  
This should only be instantiated once for the consumer you are testing.

```c#
public class ConsumerMyApiPact : IDisposable
{
  public IPactBuilder PactBuilder { get; private set; }
  public IMockProviderService MockProviderService { get; private set; }

  public int MockServerPort { get { return 9222; } }
  public string MockProviderServiceBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

  public ConsumerMyApiPact()
  {
    PactBuilder = new PactBuilder(); //Defaults to specification version 1.1.0, uses default directories. PactDir: ..\..\pacts and LogDir: ..\..\logs
    //or
    PactBuilder = new PactBuilder(new PactConfig { SpecificationVersion = "2.0.0" }); //Configures the Specification Version
    //or
    PactBuilder = new PactBuilder(new PactConfig { PactDir = @"..\pacts", LogDir = @"c:\temp\logs" }); //Configures the PactDir and/or LogDir.

    PactBuilder
      .ServiceConsumer("Consumer")
      .HasPactWith("Something API");

    MockProviderService = PactBuilder.MockService(MockServerPort); //Configure the http mock server
    //or
    MockProviderService = PactBuilder.MockService(MockServerPort, true); //By passing true as the second param, you can enabled SSL. A self signed SSL cert will be provisioned by default.
    //or
    MockProviderService = PactBuilder.MockService(MockServerPort, new JsonSerializerSettings()); //You can also change the default Json serialization settings using this overload    
  }

  public void Dispose()
  {
    PactBuilder.Build(); //NOTE: Will save the pact file once finished
  }
}

public class SomethingApiConsumerTests : IClassFixture<ConsumerMyApiPact>
{
  private IMockProviderService _mockProviderService;
  private string _mockProviderServiceBaseUri;

  public SomethingApiConsumerTests(ConsumerMyApiPact data)
  {
    _mockProviderService = data.MockProviderService;
        _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
    data.MockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
  }
}
```

#### 3. Write your test
Create a new test case and implement it.

```c#
public class SomethingApiConsumerTests : IClassFixture<ConsumerMyApiPact>
{
  private IMockProviderService _mockProviderService;
  private string _mockProviderServiceBaseUri;

  public SomethingApiConsumerTests(ConsumerMyApiPact data)
  {
    _mockProviderService = data.MockProviderService;
        _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
    data.MockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
  }

  [Fact]
  public void GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
  {
    //Arrange
    _mockProviderService
      .Given("There is a something with id 'tester'")
      .UponReceiving("A GET request to retrieve the something")
      .With(new ProviderServiceRequest
      {
        Method = HttpVerb.Get,
        Path = "/somethings/tester",
        Headers = new Dictionary<string, object>
        {
          { "Accept", "application/json" }
        }
      })
      .WillRespondWith(new ProviderServiceResponse
      {
        Status = 200,
        Headers = new Dictionary<string, object>
        {
          { "Content-Type", "application/json; charset=utf-8" }
        },
        Body = new //NOTE: Note the case sensitivity here, the body will be serialised as per the casing defined
        {
          id = "tester",
          firstName = "Totally",
          lastName = "Awesome"
        }
      }); //NOTE: WillRespondWith call must come last as it will register the interaction

    var consumer = new SomethingApiClient(_mockProviderServiceBaseUri);

    //Act
    var result = consumer.GetSomething("tester");

    //Assert
    Assert.Equal("tester", result.id);

    _mockProviderService.VerifyInteractions(); //NOTE: Verifies that interactions registered on the mock provider are called once and only once
  }
}
```

#### 4. Run the test
Everything should be green

Note: we advise using a TDD approach when using this library, however we will leave it up to you.  
Likely you will be creating a skeleton client, describing the pact, write the failing test, implement the skeleton client, run the test to make sure it passes, then rinse and repeat.

### Service Provider

#### 1. Create the API
You can create the API using whatever framework you like, however this example will use ASP.NET Web API 2 with Owin.

Note: We have removed to support for Microsoft.Owin.Testing, as we have moved to using a shared Pact core. You will now be required to start the API and listen on the correct port, as part of the test.

#### 2. Tell the provider it needs to honour the pact
Create a new test case within your service provider test project, using whatever test framework you like (in this case we used xUnit).

```c#
public class SomethingApiTests
{
  [Fact]
  public void EnsureSomethingApiHonoursPactWithConsumer()
  {
    //Arrange
    const string serviceUri = "http://localhost:9222";
    var config = new PactVerifierConfig
    {
        Outputters = new List<IOutput> //NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture the console output, so a custom outputter is required.
        {
            new XUnitOutput(_output)
        }
    };

    using (WebApp.Start<TestStartup>(serviceUri))
    {
        //Act / Assert
        IPactVerifier pactVerifier = new PactVerifier(config);
        pactVerifier
            .ProviderState($"{serviceUri}/provider-states")
            .ServiceProvider("Something API", serviceUri)
            .HonoursPactWith("Consumer")
            .PactUri("..\\..\\..\\Consumer.Tests\\pacts\\consumer-something_api.json")
            //or
            .PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest") //You can specify a http or https uri
            //or
            .PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest", new PactUriOptions("someuser", "somepassword")) //You can also specify http/https basic auth details
            .Verify();
    }
  }
}
```

```c#
public class TestStartup
{
    public void Configuration(IAppBuilder app)
    {
        var apiStartup = new Startup(); //This is your standard OWIN startup object
        app.Use<ProviderStateMiddleware>();

        apiStartup.Configuration(app);
    }
}
```

```c#
public class ProviderStateMiddleware
{
    private const string ConsumerName = "Event API Consumer";
    private readonly Func<IDictionary<string, object>, Task> m_next;
    private readonly IDictionary<string, Action> _providerStates;

    public ProviderStateMiddleware(Func<IDictionary<string, object>, Task> next)
    {
        m_next = next;
        _providerStates = new Dictionary<string, Action>
        {
            {
                "There is a something with id 'tester'",
                AddTesterIfItDoesntExist
            }
        };
    }

    private void AddTesterIfItDoesntExist()
    {
      //Add code to go an inject or insert the tester data
    }

    public async Task Invoke(IDictionary<string, object> environment)
    {
        IOwinContext context = new OwinContext(environment);

        if (context.Request.Path.Value == "/provider-states")
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method == HttpMethod.Post.ToString() &&
                context.Request.Body != null)
            {
                string jsonRequestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = reader.ReadToEnd();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                //A null or empty provider state key must be handled
                if (providerState != null &&
                    !IsNullOrEmpty(providerState.State) &&
                    providerState.Consumer == ConsumerName)
                {
                    _providerStates[providerState.State].Invoke();
                }

                await context.Response.WriteAsync(Empty);
            }
        }
        else
        {
            await m_next.Invoke(environment);
        }
    }
}
```

```c#
public class XUnitOutput : IOutput
{
    private readonly ITestOutputHelper _output;

    public XUnitOutput(ITestOutputHelper output)
    {
        _output = output;
    }

    public void WriteLine(string line)
    {
        _output.WriteLine(line);
    }
}
```

#### 4. Run the test
Everything should be green

Again, please note: we advise using a TDD approach when using this library, however we will leave it up to you.

For further examples please refer to the [Samples](https://github.com/SEEK-Jobs/pact-net/tree/master/Samples) in the solution.

#### Related Tools

You might also find the following tool and library helpful:

* [Pact based service simulator](https://github.com/seek-oss/seek.automation.phantom): leverage pacts to isolate your services
* [Pact based stubbing library](https://github.com/seek-oss/seek.automation.stub): leverage pacts during automation to stub dependencies
