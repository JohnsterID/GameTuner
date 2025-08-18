namespace GameTuner.Framework
{
	public interface IExploreFileTree
	{
		string BaseDirectory { get; set; }

		void RebuildTree();
	}
}
