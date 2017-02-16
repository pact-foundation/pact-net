using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Matchers.Include
{
	public class IncludeMatchDefinition : MatchDefinition
	{
		public const string Name = "include";

		public IncludeMatchDefinition(object example) :
			base(Name, example)
		{

		}
	}
}
