using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Models
{
    public class PactFileTests
    {
        [Fact]
        public void Ctor_WhenInstantiated_SetsPactSpecificationVersionMetaDataTo1()
        {
            var pactFile = new PactFile();

            Assert.Equal("1.0.0", pactFile.Metadata.PactSpecificationVersion);
        }
    }
}
