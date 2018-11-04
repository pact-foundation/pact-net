using Newtonsoft.Json;
using PactNet.PactMessage;

namespace PactNet
{
	public interface IPactMessageBuilder : IPactBaseBuilder<IPactMessageBuilder>
	{
		IPactMessage PactMessage(JsonSerializerSettings jsonSerializerSettings = null);
	}
}
