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
	}
}
