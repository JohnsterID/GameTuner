namespace GameTuner.Framework.Graph
{
	public interface IFlowGraphDynamicEntries
	{
		int EntryCount { get; }

		string GetEntryLabel(int index);

		void InsertEntry(int index);

		void RemoveEntry(int index);
	}
}
