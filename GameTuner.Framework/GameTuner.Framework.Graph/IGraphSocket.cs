namespace GameTuner.Framework.Graph
{
	public interface IGraphSocket : IUniqueID, ITagProvider
	{
		GraphNubCollection Nubs { get; }

		IGraphNode Owner { get; }
	}
}
