using System.Collections.Generic;
using PactNet.Models;

namespace PactNet
{
	internal interface IPactMerger
	{
		void DeleteUnexpectedInteractions(IEnumerable<Interaction> interactions, string consumer, string provider);
	}
}
