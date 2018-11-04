using PactNet.Core;
using PactNet.PactMessage.Host;
using Xunit;

namespace PactNet.Tests.Core
{
	public class PactMessageHostConfigTests
	{
		private IPactCoreHostConfig GetSubject(string arguments = "help", PactConfig pactConfig = null)
		{
			return new PactMessageHostConfig(pactConfig ?? new PactConfig(), arguments, true);
		}

		[Fact]
		public void Ctor_WhenCalled_SetsTheCorrectArgs()
		{
			var config = GetSubject();

			Assert.Equal("pact-message", config.Script);
			Assert.Equal("help", config.Arguments);
			Assert.True(config.WaitForExit);
		}
	}
}
