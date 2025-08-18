namespace GameTuner.Framework
{
	public interface ISearcherResults
	{
		void AddMatch(string location, string brief, object context);
	}
}
