namespace GameTuner.Framework
{
	public interface ISearcher
	{
		void Match(ISearcherResults results, SearchSettings settings);

		void Inspect(object context);
	}
}
