using Newtonsoft.Json;

namespace PactNet.PactMessage.Models
{
	public class ProviderState
	{
		[JsonProperty(PropertyName = "name")]
		public string State { get; set; }
	}
}