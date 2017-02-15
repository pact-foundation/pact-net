using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Include
{
	public class IncludeMatcher : IMatcher
	{

		public IncludeMatcher()
		{
		}

		public string Type
		{
			get	{ return IncludeMatchDefinition.Name; }
		}

		public MatcherResult Match(string path, JToken expected, JToken actual)
		{
			var act = actual as JValue;
			var exp = expected as JValue;
			
			if (act == null)
				return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotExist, "Include", "(null)"));

			var matches = act != null && act.Value.ToString().Contains(exp.Value.ToString());

			return matches ?
				new MatcherResult(new SuccessfulMatcherCheck(path, "Include", act.Value)) :
				new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotMatch, exp.Value.ToString(), act.Value));
		}
	}
}
