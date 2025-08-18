namespace GameTuner.Framework.Graph
{
	public class DefaultStateSocket : IGraphSocket, IUniqueID, ITagProvider, IStateGraphSocket
	{
		public GraphNubCollection Nubs { get; private set; }

		public IGraphNode Owner { get; private set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public bool Bidirectional { get; set; }

		public int Order { get; set; }

		public IGraphNode Node { get; private set; }

		public Factory<IGraphNub> NubFactory { get; private set; }

		public DefaultStateSocket(IGraphNode owner)
		{
			Owner = owner;
			Nubs = new GraphNubCollection();
			NubFactory = new Factory<IGraphNub>();
			NubFactory.Add(new Maker<DefaultStateNub>(this));
		}

		public void Connect(IGraphNode node)
		{
			Node = node;
		}

		public void Disconnect()
		{
			Node = null;
		}

		public IGraphNub CreateNub()
		{
			return new DefaultStateNub(this);
		}
	}
}
