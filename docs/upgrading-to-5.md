Upgrading to PactNet 5.x
========================

PactNet 5.x contains some breaking changes to improve the ergonomics of working with the new capabilities of Pact
Specification v4.

Obsolete Methods
----------------

The `IMessagePact` interface and `MessagePact` implementations were marked obsolete in PactNet 4.x and have been removed
in 5.0. The `IPact` and `Pact` implementation should be used instead.

The extension methods for `IPact.UsingNativeBackend` and `IMessagePact.UsingNativeBackend` were marked obsolete in PactNet
4.x and have been removed in 5.0. They have been replaced with `IPact.WithHttpInteractions` and `IPact.WithMessageInteractions`
respectively.

Combined Pacts
--------------

See: [RFC 457](https://github.com/pact-foundation/pact-net/issues/457)

In Pact Specification v3, the concept of asynchronous messaging pacts was introduced and support was added in PactNet 4.x
for this feature. This allowed for interactions to be defined in which two components communicated asynchronously, typically
through a message broker.

One key limitation of messaging support in v3 is that a Pact file can only contain either HTTP interactions or messaging
interactions, but not both. This was awkward to deal with in practice, because if a single component performed some HTTP
and some messaging interactions with another component, from the perspective of Pact these had to be treated as if they
were two separate components (e.g. an "Orders API" and an "Orders Messaging" consumer).

This artificial separation creates further problems - e.g. two separate verifications must be performed and two checks for
'Can I Deploy?' would be required when deploying one single provider.

In Pact Specification v4 this limitation has been lifted. The same Pact file can now contain both HTTP and messaging
interactions, and so the verifier needs to be changed so that it can be configured to verify both.

In [RFC 457](https://github.com/pact-foundation/pact-net/issues/457) this change was proposed such that the verifier could
configure both a HTTP endpoint and a messaging endpoint (or 'transport' as the FFI internals call them).

This breaking change means that instead of this:

```csharp
// HTTP style
var httpVerifier = new PactVerifier();
httpVerifier.ServiceProvider("My API", new Uri("http://localhost:5000"))
            .WithFileSource(...)
            .Verify();;

// Messaging style
var messageVerifier = new PactVerifier();
messageVerifier.MessagingProvider("My Messaging")
               .WithProviderMessages(scenarios => { ... })
               .WithFileSource(...)
               .Verify();;
```

You can now verify both with a single verifier:

```csharp
var verifier = new PactVerifier("My API");

verifier
    .WithHttpEndpoint(new Uri("http://localhost:5000"))
    .WithMessages(scenarios =>
    {
        scenarios.Add<MyEvent>("an event happens")
    })
    .WithFileSource(new FileInfo(@"..."))
    .Verify();
```

Replace Newtonsoft with System.Text.Json
----------------------------------------

See: [RFC 458](https://github.com/pact-foundation/pact-net/issues/458)

The dominant web framework in .Net is now ASP.Net Core, which uses the `System.Text.Json` serialiser by default instead of the
previously-favoured `Newtonsoft.Json` library. In order to reduce friction when using the now-default JSON serialiser, PactNet
now uses `System.Text.Json` for serialisation.

The main implication of this change is that any API previously referencing `JsonSerializerSettings` will now use `JsonSerializerOptions`
instead. Whenever PactNet serialises one of your types (e.g. during message interaction definitions) it will now be serialised using
`System.text.Json` instead of `Newtonsoft`, and so you may need to update any annotations on your types. Previously you likely had to
annotate your types with both types of annotation so that both ASP.Net Core and PactNet could serialise them properly, whereas now only
the `System.Text.Json` annotations will be needed in the vast majority of cases.

For example, the following messaging interaction test has the same API as before but is a breaking change because it will now use
`System.Text.Json` to deserialise:

```csharp
[Fact]
public async Task OnMessageAsync_OrderCreated_HandlesMessage()
{
    await this.pact
              .ExpectsToReceive("an event indicating that an order has been created")
              // BREAKING CHANGE: This will be serialised to the Pact file using System.Text.Json
              .WithJsonContent(new
              {
                  Id = Match.Integer(1)
              })
              // BREAKING CHANGE: OrderCreatedEvent will now be deserialised from the message definition above using System.Text.Json
              .VerifyAsync<OrderCreatedEvent>(async message =>
              {
                  await this.consumer.OnMessageAsync(message);

                  this.mockService.Verify(s => s.FulfilOrderAsync(message.Id));
              });
}
```

When verifying messages, the serialisation will also use `System.Text.Json`:

```csharp
var verifier = new PactVerifier("My API");

verifier.WithMessages(scenarios =>
        {
            // BREAKING CHANGE: The messaging response body will be serialised using System.Text.Json
            scenarios.Add<MyEvent>("an event happens")
        })
        .WithFileSource(new FileInfo(@"..."))
        .Verify();
```

Minimum Supported .Net Framework Version
----------------------------------------

The minimum supported version of .Net Framework is now 4.6.2 instead of 4.6.1 in line with the minimum supported version in
`System.Text.Json`.

MacOS ARM64 Full Support
------------------------

MacOS now has full x86-64 and ARM support.
