namespace GameTuner.Framework.Graph
{
	public interface IGraph : IUniqueID, ITagProvider, IServiceProviderProvider
	{
		GraphNodeCollection Nodes { get; }

		void RemoveLinks(IGraphNode node);
	}
}
