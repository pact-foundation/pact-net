# PactNet
[![Build status](https://ci.appveyor.com/api/projects/status/5h4t9oerlhqcnwm8/branch/master?svg=true)](https://ci.appveyor.com/project/SEEKJobs/pact-net/branch/master)  
A .NET implementation of the Ruby consumer driven contract library, [Pact](https://github.com/pact-foundation/pact-ruby).  
Pact is based off the specification found at https://github.com/pact-foundation/pact-specification.  

PactNet primarily provides a fluent .NET DSL for describing performing contract testing using Pact. See https://docs.pact.io/ for more info about Pact, the benefits it provides and the problems it solves.

PactNet is Version 2.0 compliant, and we now use the [Ruby standalone engine](https://github.com/pact-foundation/pact-ruby-standalone) as we move towards a common core approach. To enable Version 2.0 support,  make sure you supply a `PactConfig` object with `SpecificationVersion = "2.0.0"` when creating the `PactBuilder`.  

In reaching Version 2.0 compliance, we have made some breaking changes. This readme details the current latest version.  
See [Version 1.0 readme](https://github.com/pact-foundation/pact-net/blob/master/README_v1.md) for the previous version.  

Please feel free to contribute, we do accept pull requests. This solution has been built using VS2017, you will need it to open this project.

## History
PactNet was initially built at [SEEK](https://www.seek.com.au/) to help solve some of the challenges faced with testing across service boundaries.  
The project now lives in the pact-foundation GH organisation, to help group and support the official Pact libraries.  
Massive thanks to the SEEK team for all the time and hard work put into this library.

[![SEEK](https://raw.githubusercontent.com/pact-foundation/pact-net/master/seek.png "SEEK")](https://www.seek.com.au/)


## Known Issues
1. When debugging a test locally (either consumer or provider) if you click the stop button in your test runner, it will abort the process abruptly and the ruby runtime will not get cleaned up. If you do this, kill the ruby process from your task/process manager. We recommend you play the test through to the end to avoid this issue. See https://github.com/pact-foundation/pact-net/issues/108 for more details.
2. The "metadata" section is not verified for message queue pacts. See [pact-foundation/pact-message-ruby#6](https://github.com/pact-foundation/pact-message-ruby/issues/6) for more details.
3. The "params" section of the provider states is currently not supported. See [pact-foundation/pact-message-ruby#4](https://github.com/pact-foundation/pact-message-ruby/issues/4) for more details.

## A Few Others Things to Note
1. When using `Match.Regex` you must supply a valid Ruby regular expression, as we currently use the Ruby core engine.

## Installing

Via Nuget  

**Windows**  
https://www.nuget.org/packages/PactNet.Windows  
`Install-Package PactNet.Windows`

**OSX**  
https://www.nuget.org/packages/PactNet.OSX  
`Install-Package PactNet.OSX`

**Linux x64 (64-bit)**  
https://www.nuget.org/packages/PactNet.Linux.x64  
`Install-Package PactNet.Linux.x64`

**Linux x86 (32-bit)**  
https://www.nuget.org/packages/PactNet.Linux.x86  
`Install-Package PactNet.Linux.x86`


## Usage
Pact-Net supports contract testing using HTTP (what pact has always done) and message semantics. 

For the [HTTP Pact docs see /docs/pact-http.md](docs/pact-http.md)  
For the [Message Pact docs see /docs/pact-message.md](docs/pact-message.md)

## Publishing Pacts to a Broker

The Pact broker is a useful tool that can be used to share pacts between the consumer and provider. In order to make this easy, below are a couple of options for publishing your Pacts to a Pact Broker.

### Using PowerShell on your build server
[Checkout this gist](https://gist.github.com/neilcampbell/bc1fb7d409425894ece0) to see an example of how you can do this.

### Using the C# client
If you use build tools like Fake and Cake, you may want create a broker publish task using the PactPublisher.

```c#
var pactPublisher = new PactPublisher("http://test.pact.dius.com.au", new PactUriOptions("username", "password"));
pactPublisher.PublishToBroker(
    "..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json",
    "1.0.2", new [] { "master" });
```

### Publishing Provider Verification Results to a Broker
This feature allows the result of the Provider verification to be pushed to the broker and displayed on the index page.
In order for this to work you must set the ProviderVersion, PublishVerificationResults and use a pact broker uri. If you do not use a broker uri no verification results will be published. See the code snippet code below.
For more info and compatibility details [refer to this](https://github.com/pact-foundation/pact_broker/wiki/Provider-verification-results).

```c#
var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");

//Assuming build number is only set in the CI environment
var config = new PactVerifierConfig
{
    ProviderVersion = !string.IsNullOrEmpty(buildNumber) ? buildNumber : null, //NOTE: This is required for this feature to work
    PublishVerificationResults = !string.IsNullOrEmpty(buildNumber)
};
IPactVerifier pactVerifier = new PactVerifier(config);
pactVerifier
    .ServiceProvider("Something Api", serviceUri)
    .HonoursPactWith("Consumer")
    .PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/latest") //NOTE: This must be a pact broker url for this feature to work
    .Verify();
```

