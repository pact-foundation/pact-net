# PactNet.xUnit

Capture the test output helper in your test class constructor

```csharp
ITestOutputHelper output;
```

and convert it to a Pact outputter when building the `PactConfig`.

```csharp
new PactConfig
{
    Outputters = new[]
    {
        output.AsPactOutput()
        }
};
```