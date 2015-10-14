using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using PactNet.Schemas.Models;
using Xunit;

namespace PactNet.Tests.Schemas
{
    public class ProviderDataSchemaTests
    {
        [Fact]
        public void AsJsonString_WhenCalled_ReturnsJsonRepresentation()
        {
            var schemaGenerator = new JSchemaGenerator();
            
            var actualSchema = new ProviderDataSchema
            {
                Description = "My description",
                ProviderState = "My provider state",
                Schema = schemaGenerator.Generate(typeof(Sample))
            };

            const string expScehma = "{\"description\":\"My description\",\"provider_state\":\"My provider state\",\"schema\":{\"type\":\"object\",\"properties\":{\"sampleproperty\":{\"type\":\"integer\"}},\"required\":[\"sampleproperty\"]}}";
            
            var actualInteractionJson = actualSchema.AsJsonString();

            Assert.Equal(expScehma, actualInteractionJson);
        }
    }

    public class Sample
    {
        [JsonProperty("sampleproperty", Required = Required.Always)] public int SampleProperty;
    }
}
