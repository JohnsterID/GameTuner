namespace GameTuner.Framework.Graph
{
	public class DefaultStateNode : IGraphNode, IUniqueID, ITagProvider, IStateGraphNode
	{
		public GraphSocketCollection Sockets { get; private set; }

		public Vec2 Location { get; set; }

		public string Text { get; set; }

		public IGraph Owner { get; private set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public DefaultStateNode(IGraph owner)
		{
			Sockets = new GraphSocketCollection();
			Text = "";
			Location = Vec2.Empty;
			Owner = owner;
		}

		public IGraphSocket CreateSocket()
		{
			return new DefaultStateSocket(this);
		}
	}
}
