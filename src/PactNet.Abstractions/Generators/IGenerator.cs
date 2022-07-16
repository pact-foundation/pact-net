using Newtonsoft.Json;
using PactNet.Matchers;

namespace PactNet.Generators;

public interface IGenerator : IMatcher
{
    /// <summary>
    /// Type of the generator
    /// </summary>
    [JsonProperty("pact:generator:type")]
    string GeneratorType { get; }
}
