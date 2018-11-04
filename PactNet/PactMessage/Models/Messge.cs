using Newtonsoft.Json;

namespace PactNet.PactMessage.Models
{
	public class Messge
	{
		[JsonProperty(PropertyName = "contents")]
		public dynamic Contents
		{
			get; set;
		}

		[JsonProperty(PropertyName = "metadata")]
		public string Metadata
		{
			get; set;
		}
	}
}
