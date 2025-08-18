using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Matchers
{
	public class MatcherWildCard : IMatcher
	{
		public bool Match(SearchSettings settings, string matchWith)
		{
			return false;
		}

		public override string ToString()
		{
			return GameTuner.Framework.Properties.Resources.MatcherWildCard;
		}
	}
}
