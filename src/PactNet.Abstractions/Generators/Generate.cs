namespace PactNet.Generators;

public static class Generate
{
    /// <summary>
    /// Generates a value that is looked up from the provider state context using the given expression
    /// </summary>
    /// <param name="example">Example value</param>
    /// <param name="expression">String expression</param>
    /// <returns>Generator</returns>
    public static IGenerator ProviderState(string example, string expression)
    {
        return new ProviderStateGenerator(example, expression);
    }
}
