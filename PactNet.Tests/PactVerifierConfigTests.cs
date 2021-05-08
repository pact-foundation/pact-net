using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PactNet.Tests
{
    public class PactVerifierConfigTests
    {
        [Fact]
        public void CustomHeader_backwardCompatibility_getValue()
        {
            var config = new PactVerifierConfig();

            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string> ( "key name", "value");

#pragma warning disable CS0618 // Type or member is obsolete
            config.CustomHeader = keyValuePair;

            Assert.Equal(config.CustomHeader, keyValuePair);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.True(config.CustomHeaders.Contains(keyValuePair));
            Assert.Equal(config.CustomHeaders.Count, 1);

        }

        [Fact]
        public void CustomHeader_backwardCompatibility()
        {
            var config = new PactVerifierConfig();

            KeyValuePair<string, string> oldKeyValuePair = new KeyValuePair<string, string>("key name", "value");

            // Expect an initial length of 0
            Assert.Equal(config.CustomHeaders.Count, 0);

#pragma warning disable CS0618 // Type or member is obsolete
            config.CustomHeader = oldKeyValuePair;
            // confirm the value was added
            Assert.Equal(config.CustomHeaders.Count, 1);
            config.CustomHeader = new KeyValuePair<string, string>("new key name", "new value");
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.NotEqual(config.CustomHeader, oldKeyValuePair);
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.False(config.CustomHeaders.Contains(oldKeyValuePair));
            Assert.Equal(config.CustomHeaders.Count, 1);

        }
    }
}
