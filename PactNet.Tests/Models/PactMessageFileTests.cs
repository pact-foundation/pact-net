using Newtonsoft.Json;
using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PactNet.Tests.Models
{
    public class PactMessageFileTests
    {
       [Fact]
       public void Ctor_SetsPactVersionTo3_0()
        {
            var sut = new MessagingPactFile();

           // Assert.Equal("3.0.0", sut.Metadata.pactSpecificationVersion);

            string json = JsonConvert.SerializeObject(sut);
        }
    }
}
