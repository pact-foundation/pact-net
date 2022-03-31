using Newtonsoft.Json;

namespace PactNet.Generators;

public class ProviderStateGenerator : IGenerator
{
    public string Type => "type";

    public dynamic Value { get; }

    public string GeneratorType => "ProviderState";

    /// <summary>
    /// Expression to lookup in provider state context
    /// </summary>
    [JsonProperty("expression")]
    public string Expression { get; }

    public ProviderStateGenerator(dynamic example, string expression)
    {
        this.Value = example;
        this.Expression = expression;
    }
}
