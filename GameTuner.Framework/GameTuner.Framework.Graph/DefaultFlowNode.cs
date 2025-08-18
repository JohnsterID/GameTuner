using System;

namespace GameTuner.Framework.Graph
{
	public class DefaultFlowNode : IGraphNode, IUniqueID, ITagProvider, IFlowGraphNode
	{
		public GraphSocketCollection Sockets { get; private set; }

		public Vec2 Location { get; set; }

		public string Text { get; set; }

		public IGraph Owner { get; set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public GraphSocketCollection InputSockets { get; private set; }

		public GraphSocketCollection OutputSockets { get; private set; }

		public DefaultFlowNode(IGraph owner)
		{
			Sockets = new GraphSocketCollection();
			InputSockets = new GraphSocketCollection();
			OutputSockets = new GraphSocketCollection();
			Owner = owner;
		}

		public bool IsOfType(IGraphSocket socket, FlowGraphSocketType type)
		{
			if (type != FlowGraphSocketType.Input)
			{
				return OutputSockets.Contains(socket);
			}
			return InputSockets.Contains(socket);
		}

		public IGraphSocket Add(string name, Type socketType, FlowGraphSocketType type)
		{
			return Add(name, socketType, type, 0);
		}

		public IGraphSocket Add(string name, Type socketType, FlowGraphSocketType type, int id)
		{
			IGraphSocket graphSocket = new DefaultFlowSocket(this, socketType, name);
			graphSocket.ID = id;
			Sockets.Add(graphSocket);
			GraphSocketCollection graphSocketCollection = ((type == FlowGraphSocketType.Input) ? InputSockets : OutputSockets);
			graphSocketCollection.Add(graphSocket);
			return graphSocket;
		}

		public IGraphSocket Find(string name)
		{
			foreach (IGraphSocket socket in Sockets)
			{
				if (string.Compare(((IFlowGraphSocket)socket).Name, name) == 0)
				{
					return socket;
				}
			}
			return null;
		}
	}
}
