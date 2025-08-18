using System.Text.RegularExpressions;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Matchers
{
	public class MatcherRegEx : IMatcher
	{
		public bool Match(SearchSettings settings, string matchWith)
		{
			Match match = Regex.Match(matchWith, settings.SearchValue, (!settings.MatchCase) ? RegexOptions.IgnoreCase : RegexOptions.None);
			return match.Success;
		}

		public override string ToString()
		{
			return GameTuner.Framework.Properties.Resources.MatcherRegEx;
		}
	}
}
