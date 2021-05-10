using PactNet.Infrastructure.Outputters;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PactNet.Tests
{
    public class PactVerifierConfigTests
    {
        [Fact]
        public void PactVertifier_Init_State_CustomHeaderIsNull()
        {
            var config = new PactVerifierConfig();

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Null(config.CustomHeader);
#pragma warning restore CS0618 // Type or member is obsolete

        }

        [Fact]
        public void PactVertifier_Init_State_OutputersIsNotNull()
        {
            var config = new PactVerifierConfig();

            Assert.NotNull( config.Outputters );

        }

        [Fact]
        public void PactVertifier_Init_State_CustomHeadersCountIsZero()
        {
            var config = new PactVerifierConfig();

            Assert.Equal(0, config.CustomHeaders.Count);

        }

        [Fact]
        public void CustomHeader_backwardCompatibility_ConfirmKeyPairinCustomHeaders()
        {
            var config = new PactVerifierConfig();

            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";

#pragma warning disable CS0618 // Type or member is obsolete
            // set the CustomHeader value
            config.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);

            // Confirm CustomHeaders contains the supplied Key
            Assert.True(config.CustomHeaders.ContainsKey(config.CustomHeader.Value.Key));
#pragma warning restore CS0618 // Type or member is obsolete

        }

        [Fact]
        public void CustomHeader_backwardCompatibility_SetCustomHeaderToNull_ConfirmCustomHeadersCountIsZero()
        {

            var config = new PactVerifierConfig();

#pragma warning disable CS0618 // Type or member is obsolete
            // set the CustomHeader value
            config.CustomHeader = null;
#pragma warning restore CS0618 // Type or member is obsolete

            // Confirm CustomHeaders contains the supplied Key
            Assert.Equal(0 ,config.CustomHeaders.Count);
        }

        [Fact]
        public void CustomHeader_backwardCompatibility_AddCustomHeaderAndConfirmCustomHeadersSizeIsOne()
        {
            var config = new PactVerifierConfig();

            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";

#pragma warning disable CS0618 // Type or member is obsolete
            // set the CustomHeader value
            config.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);
#pragma warning restore CS0618 // Type or member is obsolete

            // check the action has added one CustomHeader
            Assert.Equal(1, config.CustomHeaders.Count);

        }

        [Fact]
        public void CustomHeader_backwardCompatibility_ChangeCustomHeader_ConfirmCustomHeadersSizeIsOne()
        {
            var config = new PactVerifierConfig();

            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";
            string prefix = "new";

#pragma warning disable CS0618 // Type or member is obsolete
            // set the CustomHeader value
            config.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);
            config.CustomHeader = new KeyValuePair<string, string>(prefix + dummy_key, prefix + dummy_value);
#pragma warning restore CS0618 // Type or member is obsolete

            // check the action has added one CustomHeader
            Assert.Equal(1, config.CustomHeaders.Count);

        }
    }
}
