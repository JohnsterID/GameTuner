namespace GameTuner.Framework.Graph
{
	public interface IGraphNode : IUniqueID, ITagProvider
	{
		GraphSocketCollection Sockets { get; }

		Vec2 Location { get; set; }

		string Text { get; set; }

		IGraph Owner { get; }
	}
}
