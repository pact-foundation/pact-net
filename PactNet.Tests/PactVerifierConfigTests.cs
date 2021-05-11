using System.Collections.Generic;
using Xunit;

namespace PactNet.Tests
{
    public class PactVerifierConfigTests
    {
        readonly PactVerifierConfig _verifierConfig = null;
        public PactVerifierConfigTests()
        {
             _verifierConfig = new PactVerifierConfig();
        }

        [Fact]
        [System.Obsolete]
        public void PactVertifier_Init_State_CustomHeaderIsNull()
        {

            Assert.Null(_verifierConfig.CustomHeader);

        }

        [Fact]
        public void PactVertifier_Init_State_OutputersIsNotNull()
        {
            Assert.NotNull( _verifierConfig.Outputters );
        }

        [Fact]
        public void PactVertifier_Init_State_CustomHeadersCountIsZero()
        {
            Assert.Equal(0, _verifierConfig.CustomHeaders.Count);
        }

        [Fact]
        [System.Obsolete]
        public void CustomHeader_backwardCompatibility_ConfirmKeyPairinCustomHeaders()
        {
            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";

            _verifierConfig.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);

            Assert.True(_verifierConfig.CustomHeaders.ContainsKey(_verifierConfig.CustomHeader.Value.Key));

        }

        [Fact]
        [System.Obsolete]
        public void CustomHeader_backwardCompatibility_SetCustomHeaderToNull_ConfirmCustomHeadersCountIsZero()
        {
            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";

            _verifierConfig.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);

            _verifierConfig.CustomHeader = null;

            Assert.Equal(0 , _verifierConfig.CustomHeaders.Count);
        }

        [Fact]
        [System.Obsolete]
        public void CustomHeader_backwardCompatibility_AddCustomHeaderAndConfirmCustomHeadersSizeIsOne()
        {
            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";

            _verifierConfig.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);

            Assert.Equal(1, _verifierConfig.CustomHeaders.Count);

        }

        [Fact]
        [System.Obsolete]
        public void CustomHeader_backwardCompatibility_ChangeCustomHeader_ConfirmCustomHeadersSizeIsOne()
        {
            string dummy_key = "dummyKey";
            string dummy_value = "dummyValue";
            string prefix = "new";

            _verifierConfig.CustomHeader = new KeyValuePair<string, string>(dummy_key, dummy_value);
            _verifierConfig.CustomHeader = new KeyValuePair<string, string>(prefix + dummy_key, prefix + dummy_value);

            // check the action has added one CustomHeader
            Assert.Equal(1, _verifierConfig.CustomHeaders.Count);

        }
    }
}
