using System;

namespace GameTuner.Framework.Graph
{
	public class DefaultFlowSocket : IGraphSocket, IUniqueID, ITagProvider, IFlowGraphSocket
	{
		private IGraphNode node;

		private IGraphSocket outputSocket;

		public GraphNubCollection Nubs { get; private set; }

		public IGraphNode Owner { get; private set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public string Name { get; set; }

		public FlowGraphSocketType FlowType { get; private set; }

		public IGraphNode Node
		{
			get
			{
				return node;
			}
		}

		public IGraphSocket OutputSocket
		{
			get
			{
				return outputSocket;
			}
		}

		public Type SocketType { get; private set; }

		public DefaultFlowSocket(IGraphNode owner, Type type, string name)
		{
			Owner = owner;
			SocketType = type;
			Name = name;
			FlowType = FlowGraphSocketType.None;
		}

		public void Connect(IGraphNode node, IGraphSocket socket)
		{
			this.node = node;
			outputSocket = socket;
		}

		public void Disconnect()
		{
			node = null;
			outputSocket = null;
		}
	}
}
