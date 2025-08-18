namespace GameTuner.Framework
{
	public interface IMatcher
	{
		bool Match(SearchSettings settings, string matchWith);
	}
}
