# Pact Message
These docs describe how you would use Pact-Net to test a message provider (aka producers or publisher) and message consumers.
Asynchronous messaging is a common paradigm used in distributed architecures, using tools such as RabbitMQ, SQS, Kafka and Kinesis (to name a few).

Supporting all these different tools and libraries would be a big task, so Pact ignores the underlying protocol and focusses on message passing and the messages themselves.
For more information about this [see this article](https://dius.com.au/2017/09/22/contract-testing-serverless-and-asynchronous-applications/).

## Usage

Below are some samples of usage.  

### Message Consumer
A message consumer is the code that handles a message.  
Pact will invoke your handler with the expected message, so you can validate your handler works correctly.

#### 1. Build your message consumer
In order to make a message consumer testable with Pact, we need to isolate the message queue/protocol/infrastructure bindings (message listener) from the message handling code (message handler).  
Pact only needs to validate the message handlers and therefore this readme doesn't list any message listener code.

A message handler might look like below:

```c#
public class SavePets
{
    private readonly IPetRepo _repo;

    public SavePets(IPetRepo repo)
    {
        _repo = repo;
    }

    public void Handle(AnimalCreated @event)
    {
        var isPet = Enum.TryParse(@event.Type, true, out PetType petType);
        if (isPet)
        {
            _repo.SavePet(new Pet
            {
                Id = @event.Id,
                Name = @event.Name,
                Type = petType
            });
        }
    }
}
```

#### 2. Describe and configure the pact
Create a new test case within your message consumer test project, using whatever test framework you like (in this case we used xUnit).
This should only be instantiated once for the message consumer you are testing.

```c#
public class ConsumerEventPact : IDisposable
{
    private readonly IMessagePactBuilder _messagePactBuilder;
    public IMessagePact MessagePact;

    public ConsumerEventPact(IMessageSink sink)
    {
        _messagePactBuilder = new MessagePactBuilder(new PactConfig
            {
                SpecificationVersion = "2.0.0",
                LogDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}",
                PactDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}",
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(sink)
                }
            })
            .ServiceConsumer("Zoo Event Consumer")
            .HasPactWith("Zoo Event Producer");

        MessagePact = _messagePactBuilder.InitializePactMessage();
    }

    public void Dispose()
    {
        _messagePactBuilder.Build(); //NOTE: Will save the pact file once finished
    }
}
```

#### 3. Write your test
Create a new test case and implement it.

```c#
public class SavePetsTests : IClassFixture<ConsumerEventPact>
{
    private readonly IMessagePact _messagePact;

    public SavePetsTests(ConsumerEventPact data)
    {
        _messagePact = data.MessagePact;
    }

    [Fact]
    public void Handle_WhenAPetIsCreated_SavesThePet()
    {
        //Arrange
        var stubRepo = Substitute.For<IPetRepo>();
        var consumer = new SavePets(stubRepo);
        var pet = new Pet { Id = 1, Name = "Rover", Type = PetType.Dog };

        var providerStates = new[]
        {
            new ProviderState
            {
                Name = "there is a Pet animal"
            }
        };

        //Act + Assert
        _messagePact.Given(providerStates)
            .ExpectedToReceive("a pet animal created event")
            .With(new Message
            {
                Contents = new
                {
                    id = Match.Type(pet.Id),
                    name = Match.Type(pet.Name),
                    type = Match.Regex(pet.Type.ToString(), "^(Dog|Cat|Fish)$")
                }
            })
            .VerifyConsumer<AnimalCreated>(e => consumer.Handle(e)); //This also checks that no exceptions are thrown
        stubRepo.Received(1).SavePet(pet);
    }
}
```

#### 4. Run the test
If you now navigate to [RepositoryRoot]/pacts you will see the pact file your test generated. Take a moment to have a look at what it contains which is a JSON representation of the messages your test handled.

Everything should be green.

### Message Provider (Producer) 

A message provider (or producer) is the system that puts the message onto the message infrastructure (queue, stream etc).  
Pact checks that the message sent from the provider matches the consumers expectations.

#### 1. Create the message provider
Similar to the consumer, we need to isolate the message sending queue/protocol/infrastructure bindings (message sender) from the message generation code (message generator).
Pact only needs to validate the message generators and therefore this readme doesn't list any message sender code.

A message generator might look like below:

```c#
public class AnimalCreator
{
    private int _currentId = 1;

    //This is the message generator, which contains any message generation domain logic.

    public AnimalCreated CreateAPet(string name, PetType type)
    {
        return new AnimalCreated
        {
            Id = _currentId++,
            Name = name,
            Type = type.ToString()
        };
    }
}
```

#### 2. Tell the message provider it needs to honour the pact
Create a new test case within your message provider test project, using whatever test framework you like (in this case we used xUnit). PactUri method should refer to the pact json file, created when the message consumer test was run.

```c#
public class ZooProducerTests
{
    private readonly ITestOutputHelper _output;

    public ZooProducerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void EnsureEventProducerHonoursPactWithEventConsumer()
    {
        //Arrange
        const string serviceUri = "http://localhost:9222";
        var config = new PactVerifierConfig
        {
            Outputters = new List<IOutput>
            {
                new XUnitOutput(_output)
            }
        };

        var builder = WebHost.CreateDefaultBuilder()
            .UseUrls(serviceUri)
            .PreferHostingUrls(true)
            .UseStartup<TestStartup>();

        using (var host = builder.Build())
        {
            host.Start();
        
            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                    .ServiceProvider("Zoo Event Producer", serviceUri)
                    .HonoursPactWith("Zoo Event Consumer")
                    .PactUri($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}ZooEventsConsumer.Tests{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}zoo_event_consumer-zoo_event_producer.json")
                    .Verify();
        }
    }
}
```

You may be thinking why does the serviceUri exist? I thought we were testing messages and not HTTP services? You are correct, however Pact-Net uses the Ruby core engine, which uses HTTP as the interface protocol.
The ruby Pact core calls the `/` endpoint on the supplied serviceUri, from there a mapping to the correct message generator can be provided.
Unfortunately, this needs to be implemented by you, as there is no catch all defacto standard HTTP server in .NET.

You can see the samples for a full implementation, however below is an example of the mapping.

```c#
[Route("")]
public class VerificationController : Controller
{
    private readonly MessageInvoker _messageInvoker;

    public VerificationController()
    {
        var animalCreator = new AnimalCreator();
        _messageInvoker = new MessageInvoker(new Dictionary<string, Action>
            {
                {
                    "there is a Pet animal",
                    InsertPetIntoDatabase
                }
            },
            new Dictionary<string, Func<dynamic>>
            {
                { "a pet animal created event", () => animalCreator.CreateAPet("Jane", PetType.Cat) }
            }
        );
    }

    // POST: /
    [HttpPost]
    public ActionResult<dynamic> Post([FromBody] MessagePactDescription description)
    {
        var message = _messageInvoker.Invoke(description);
        return new { contents = message };
    }

    private void InsertPetIntoDatabase()
    {
        //ImplementProviderState
    }
}
```

#### 3. Run the test
Everything should be green.

