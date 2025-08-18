using System;

namespace GameTuner.Framework.Graph
{
	public class GraphItemDescriptor : IDataTypeProvider
	{
		private int node;

		private int socket;

		private int nub;

		public GraphItemType Type { get; private set; }

		public Type DataType { get; private set; }

		public string DataName { get; private set; }

		public FlowGraphSocketType FlowType { get; private set; }

		public IGraph Graph { get; private set; }

		public IGraphNode Node
		{
			get
			{
				return Graph.Nodes.Find(node);
			}
		}

		public IGraphSocket Socket
		{
			get
			{
				IGraphNode graphNode = Node;
				if (graphNode != null)
				{
					return graphNode.Sockets.Find(socket);
				}
				return null;
			}
		}

		public IGraphNub Nub
		{
			get
			{
				IGraphSocket graphSocket = Socket;
				if (graphSocket != null)
				{
					return graphSocket.Nubs.Find(nub);
				}
				return null;
			}
		}

		public GraphItemDescriptor(IGraph graph, object item)
		{
			Type = GraphItemType.Unknown;
			Graph = graph;
			IGraphNode graphNode = item as IGraphNode;
			if (graphNode != null)
			{
				node = graphNode.ID;
				Type = GraphItemType.Node;
			}
			IGraphSocket graphSocket = item as IGraphSocket;
			if (graphSocket != null)
			{
				node = graphSocket.Owner.ID;
				socket = graphSocket.ID;
				Type = GraphItemType.Socket;
				if (graphSocket is IFlowGraphSocket)
				{
					IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)graphSocket;
					DataType = flowGraphSocket.SocketType;
					FlowType = flowGraphSocket.FlowType;
				}
			}
			IGraphNub graphNub = item as IGraphNub;
			if (graphNub != null)
			{
				node = graphNub.Owner.Owner.ID;
				socket = graphNub.Owner.ID;
				nub = graphNub.ID;
				Type = GraphItemType.Nub;
			}
			DataNameAttribute attribute = ReflectionHelper.GetAttribute<DataNameAttribute>(item);
			DataName = ((attribute != null) ? attribute.Name : item.GetType().Name);
			if (DataType == null)
			{
				DataType = ((item is IDataTypeProvider) ? ((IDataTypeProvider)item).DataType : item.GetType());
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is GraphItemDescriptor)
			{
				return Equals((GraphItemDescriptor)obj);
			}
			return false;
		}

		public bool Equals(GraphItemDescriptor other)
		{
			if (node == other.node && socket == other.socket)
			{
				return nub == other.nub;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return node.GetHashCode() + socket.GetHashCode() + nub.GetHashCode();
		}

		public object Create()
		{
			object result = null;
			switch (Type)
			{
			case GraphItemType.Node:
				result = ((!(Graph is IStateGraph)) ? ((IFlowGraph)Graph).CreateNode(DataName) : ((IStateGraph)Graph).CreateNode());
				break;
			case GraphItemType.Socket:
			{
				if (Graph is IStateGraph)
				{
					result = ((IStateGraphNode)Node).CreateSocket();
					break;
				}
				IGraphNode graphNode = Node;
				if ((result = graphNode.Sockets.Find(socket)) == null)
				{
					result = ((IFlowGraphNode)graphNode).Add(DataName, DataType, FlowType, socket);
				}
				break;
			}
			case GraphItemType.Nub:
				if (Graph is IStateGraph)
				{
					result = ((IStateGraphSocket)Socket).NubFactory.Make(DataName, Socket);
					break;
				}
				throw new NotSupportedException();
			}
			return result;
		}

		public void Add(object item)
		{
			switch (Type)
			{
			case GraphItemType.Nub:
				Socket.Nubs.Add((IGraphNub)item);
				break;
			case GraphItemType.Socket:
			{
				IGraphSocket graphSocket = (IGraphSocket)item;
				if (!(graphSocket is IFlowGraphSocket))
				{
					Node.Sockets.Add(graphSocket);
				}
				break;
			}
			case GraphItemType.Node:
				Graph.Nodes.Add((IGraphNode)item);
				break;
			}
		}

		public void Remove()
		{
			switch (Type)
			{
			case GraphItemType.Nub:
				Socket.Nubs.Remove(Nub);
				break;
			case GraphItemType.Socket:
			{
				IGraphSocket graphSocket = Socket;
				IGraphNode graphNode = Node;
				if (graphNode is IFlowGraphNode)
				{
					((IFlowGraphSocket)graphSocket).Disconnect();
				}
				else
				{
					graphNode.Sockets.Remove(graphSocket);
				}
				break;
			}
			case GraphItemType.Node:
				Graph.Nodes.Remove(Node);
				break;
			}
		}
	}
}
