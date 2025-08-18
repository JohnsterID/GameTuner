namespace GameTuner.Framework.Matchers
{
	public class MatcherBasic : IMatcher
	{
		public bool Match(SearchSettings settings, string matchWith)
		{
			if (settings.MatchWholeWord)
			{
				return string.Compare(settings.SearchValue, matchWith, !settings.MatchCase) == 0;
			}
			if (settings.MatchCase)
			{
				return matchWith.Contains(settings.SearchValue);
			}
			return matchWith.ToLower().Contains(settings.SearchValue.ToLower());
		}
	}
}
