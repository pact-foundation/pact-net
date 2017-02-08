using Newtonsoft.Json;
using PactNet.Models;
using System.Reflection;
using Xunit;
using static PactNet.Models.PactFile;

namespace PactNet.Tests.Models
{
    public class PactFileTests
    {
        [Fact]
        public void Ctor_WhenInstantiated_SetsPactSpecificationVersionMetaDataTo3()
        {
            var pactFile = new PactFile();

            Assert.Equal("3.0.0", pactFile.Metadata.PactSpecification.Version);
            Assert.Equal(pactFile.GetType().Assembly.GetName().Version.ToString(), pactFile.Metadata.PactNet.Version);

        }
    }
}
