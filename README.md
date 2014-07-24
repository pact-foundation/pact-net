# PactNet
A .NET implementation of the Ruby consumer driven contract library, [Pact](https://github.com/realestate-com-au/pact).  
Pact is based off the specification found at https://github.com/bethesque/pact_specification.  

PactNet primarily provides a fluent .NET DSL for describing HTTP requests that will be made to a service provider and the HTTP responses the consumer expects back to function correctly.  
In documenting the consumer interactions, we can replay them on the provider and ensure the provider responds as expected. This basically gives us complete test symmetry and removes the basic need for integrated tests.  
PactNet also has the ability to support other mock providers should we see fit.

PactNet is aiming to be Pact Specification Version 1 compliant (Version 2 is a WIP).  

From the [Pact Specification repo] (https://github.com/bethesque/pact_specification)

> "Pact" is an implementation of "consumer driven contract" testing that allows mocking of responses in the consumer codebase, and verification of the interactions in the provider codebase. The initial implementation was written in Ruby for Rack apps, however a consumer and provider may be implemented in different programming languages, so the "mocking" and the "verifying" steps would be best supported by libraries in their respective project's native languages. Given that the pact file is written in JSON, it should be straightforward to implement a pact library in any language, however, to get the best experience and most reliability of out mixing pact libraries, the matching logic for the requests and responses needs to be identical. There is little confidence to be gained in having your pacts "pass" if the logic used to verify a "pass" is inconsistent between implementations.

Read more about Pact and the problems it solves at https://github.com/realestate-com-au/pact

Please also note that this project is still a work in progress, so there will likely be bugs and items that have not yet been implemented.  
Please feel free to contribute, we do accept pull requests.

## Usage
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
		var client = new HttpClient();
		client.BaseAddress = new Uri(BaseUri);
		
		var request = new HttpRequestMessage(HttpMethod.Get, "/somethings/" + id);
		request.Headers.Add("Accept", "application/json");

		var response = client.SendAsync(request);

		var content = response.Result.Content.ReadAsStringAsync().Result;
		var status = response.Result.StatusCode;

		if (status == HttpStatusCode.OK)
		{
			return !String.IsNullOrEmpty(content) ? 
				JsonConvert.DeserializeObject<Something>(content)
				: null;
		}

		throw new Exception("Server responded with a non 200 status code");
	}
}
```

#### 2. Describe and configure the pact as a service consumer with a mock service
Create a new test case within your service consumer test project, using whatever test framework you like (in this case we used xUnit).  
This should only be instantiated once for the consumer you are testing.

```c#
public class ConsumerMyApiPact : IDisposable
{
	public IPactConsumer Pact { get; private set; }
	public IMockProviderService MockProviderService { get; private set; }

	public int MockServerPort { get { return 1234; } }
	public string MockServerBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

	public ConsumerMyApiPact()
	{
		Pact = new Pact().ServiceConsumer("Consumer")
			.HasPactWith("Something API");

		MockProviderService = Pact.MockService(MockServerPort); //Configure the in memory http mock server
	}

	public void Dispose()
	{
		Pact.Dispose(); //Will save the pact file once finished
	}
}

public class SomethingApiConsumerTests : IUseFixture<ConsumerMyApiPact>
{
	private ConsumerMyApiPact _data;
		
	public void SetFixture(ConsumerMyApiPact data)
	{
		_data = data;
	}
}
```

#### 3. Write your test
Create a new test case and implement it.


```c#
public class SomethingApiConsumerTests : IUseFixture<ConsumerMyApiPact>
{
	//As above...
	
	[Fact]
	public void GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
	{
		//Arrange
		_data.MockProviderService
			.Given("There is a something with id 'tester'")
			.UponReceiving("A GET request to retrieve the something")
			.With(new PactProviderServiceRequest
			{
				Method = HttpVerb.Get,
				Path = "/somethings/tester",
				Headers = new Dictionary<string, string>
				{
					{ "Accept", "application/json" }
				}
			})
			.WillRespondWith(new PactProviderServiceResponse
			{
				Status = 200,
				Headers = new Dictionary<string, string>
				{
					{ "Content-Type", "application/json; charset=utf-8" }
				},
				Body = new //Note the case sensitivity here, the body will be serialised as per the casing defined
				{
					id = "tester",
					firstName = "Totally",
					lastName = "Awesome"
				}
			}); //WillResponseWith call must come last as it will register the interaction
			
		var consumer = new SomethingApiClient(_data.MockServerBaseUri);
		
		//Act
		var result = consumer.GetSomething("tester");
		
		//Assert
		Assert.Equal("tester", result.id);
	}
}
```

#### 4. Run the test
Everything should be green

Note: we advise using a TDD approach when using this library, however we will leave it up to you.  
Likely you will be creating a skeleton client, describing the pact, write the failing test, implement the skeleton client, run the test to make sure it passes, then rinse and repeat.

### Service Provider

#### 1. Create the API
You can create the API using whatever framework you like, however we recommend using ASP.NET Web API 2 with Owin as it allows you to use the Microsoft.Owin.Testing Nuget package.  
Using the Microsoft.Owin.Testing package allows you to create an in memory version of your API to test against, which removes the pain of deployment and configuration when running the tests.

#### 2. Tell the provider it needs to honour the pact
Create a new test case within your service provider test project, using whatever test framework you like (in this case we used xUnit).

**a)** If using an Owin compatible API, like ASP.NET Web API 2 and using the Microsoft.Owin.Testing Nuget package **(preferred)**
```c#
public class SomethingApiTests
{
	[Fact]
	public void EnsureSomethingApiHonoursPactWithConsumer()
	{
		//Arrange
		var testServer = TestServer.Create<Startup>(); //Using the Microsoft.Owin.Testing nuget package

		var pact = new Pact()
			.ProviderStatesFor("Consumer",
			new Dictionary<string, Action>
			{
				{ "There is a something with id 'tester'", AddTesterIfItDoesntExist }
			});

		//Act / Assert
		pact.ServiceProvider("Something API", testServer.HttpClient)
			.HonoursPactWith("Consumer")
			.PactUri("../../../Consumer.Tests/pacts/consumer-something_api.json")
			.Verify(); //Optionally you can control what interactions are verified by specifying a providerDescription and/or providerState

		testServer.Dispose();
	}

	private void AddTesterIfItDoesntExist()
	{
		//Logic to add the 'tester' something
	}
}
```

or


**b)** If using a non Owin compatible API, you can provide your own HttpClient and point to the deployed API.
```c#
public class SomethingApiTests
{
	[Fact]
	public void EnsureSomethingApiHonoursPactWithConsumer()
	{
		//Arrange
		var client = new HttpClient { BaseAddress = new Uri("http://api-address:9999") };

		var pact = new Pact()
			.ProviderStatesFor("Consumer",
			new Dictionary<string, Action>
			{
				{ "There is a something with id 'tester'", AddTesterIfItDoesntExist }
			});

		//Act / Assert
		pact.ServiceProvider("Something API", client)
			.HonoursPactWith("Consumer")
			.PactUri("../../../Consumer.Tests/pacts/consumer-something_api.json")
			.Verify(); //Optionally you can control what interactions are verified by specifying a providerDescription and/or providerState

		testServer.Dispose();
	}

	private void AddTesterIfItDoesntExist()
	{
		//Logic to add the 'tester' something
	}
}
```

#### 4. Run the test
Everything should be green

Again, please note: we advise using a TDD approach when using this library, however we will leave it up to you.

For further examples please refer to the [Samples](https://github.com/SEEK-Jobs/pact-net/tree/master/Samples) in the solution.