using System;
using System.Collections.Generic;

namespace GameTuner.Framework.Graph
{
	[UndoStyle(UndoStyle.State)]
	public class UndoGraphState : IUndo
	{
		private enum GraphType
		{
			Flow,
			State
		}

		private class SocketState
		{
			public GraphType GraphType { get; private set; }

			public string Name { get; private set; }

			public int ID { get; private set; }

			public int NodeID { get; private set; }

			public int SocketID { get; private set; }

			public Type SocketType { get; private set; }

			public FlowGraphSocketType Type { get; private set; }

			public List<UndoField> Fields { get; private set; }

			public SocketState(IGraphSocket socket, GraphType graphType)
			{
				if (graphType == GraphType.Flow)
				{
					IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)socket;
					Name = flowGraphSocket.Name;
					SocketType = flowGraphSocket.SocketType;
					Type = (((IFlowGraphNode)socket.Owner).IsOfType(socket, FlowGraphSocketType.Input) ? FlowGraphSocketType.Input : FlowGraphSocketType.Output);
					NodeID = ((flowGraphSocket.Node != null) ? flowGraphSocket.Node.ID : 0);
					SocketID = ((flowGraphSocket.OutputSocket != null) ? flowGraphSocket.OutputSocket.ID : 0);
				}
				else
				{
					IStateGraphSocket stateGraphSocket = (IStateGraphSocket)socket;
					NodeID = ((stateGraphSocket.Node != null) ? stateGraphSocket.Node.ID : 0);
				}
				Fields = UndoHelper.AcquireFields(socket);
				GraphType = graphType;
				ID = socket.ID;
			}
		}

		private class NodeState
		{
			public GraphType GraphType { get; private set; }

			public string Name { get; private set; }

			public int ID { get; private set; }

			public List<UndoField> Fields { get; private set; }

			public List<SocketState> Sockets { get; private set; }

			public NodeState(IGraphNode node, GraphType graphType)
			{
				ID = node.ID;
				DataNameAttribute attribute = ReflectionHelper.GetAttribute<DataNameAttribute>(node);
				Name = ((attribute != null) ? attribute.Name : node.GetType().Name);
				Fields = UndoHelper.AcquireFields(node);
				GraphType = graphType;
				Sockets = new List<SocketState>();
				foreach (IGraphSocket socket in node.Sockets)
				{
					Sockets.Add(new SocketState(socket, GraphType));
				}
			}
		}

		private IGraph graph;

		private GraphType graphType;

		private List<NodeState> undoStates;

		private List<NodeState> redoStates;

		public UndoGraphState(IGraph graph)
		{
			this.graph = graph;
			graphType = ((!(graph is IFlowGraph)) ? GraphType.State : GraphType.Flow);
			undoStates = new List<NodeState>();
			redoStates = new List<NodeState>();
		}

		public void PerformUndo()
		{
			Restore(undoStates);
		}

		public void PerformRedo()
		{
			Restore(redoStates);
		}

		private void Restore(List<NodeState> states)
		{
			graph.Nodes.Clear();
			IFlowGraph flowGraph = graph as IFlowGraph;
			IStateGraph stateGraph = graph as IStateGraph;
			foreach (NodeState state in states)
			{
				IGraphNode graphNode = ((graphType == GraphType.Flow) ? flowGraph.CreateNode(state.Name, state.ID) : stateGraph.CreateNode());
				UndoHelper.ApplyFields(graphNode, state.Fields);
				graph.Nodes.Add(graphNode);
				foreach (SocketState socket in state.Sockets)
				{
					if (graphType == GraphType.Flow)
					{
						IFlowGraphNode flowGraphNode = (IFlowGraphNode)graphNode;
						IGraphSocket obj = flowGraphNode.Add(socket.Name, socket.SocketType, socket.Type, socket.ID);
						UndoHelper.ApplyFields(obj, socket.Fields);
					}
				}
			}
			foreach (NodeState state2 in states)
			{
				IGraphNode graphNode2 = graph.Nodes.Find(state2.ID);
				foreach (SocketState socket2 in state2.Sockets)
				{
					IGraphSocket graphSocket = graphNode2.Sockets.Find(socket2.ID);
					if (graphType == GraphType.Flow)
					{
						IFlowGraphNode flowGraphNode2 = (IFlowGraphNode)graphNode2;
						if (flowGraphNode2.IsOfType(graphSocket, FlowGraphSocketType.Input) && socket2.NodeID != 0)
						{
							IGraphNode graphNode3 = graph.Nodes.Find(socket2.NodeID);
							IFlowGraphSocket flowGraphSocket = (IFlowGraphSocket)graphSocket;
							flowGraphSocket.Connect(graphNode3, graphNode3.Sockets.Find(socket2.SocketID));
						}
					}
				}
			}
		}

		public void StoreUndo()
		{
			foreach (IGraphNode node in graph.Nodes)
			{
				undoStates.Add(new NodeState(node, graphType));
			}
		}

		public void StoreRedo()
		{
			foreach (IGraphNode node in graph.Nodes)
			{
				redoStates.Add(new NodeState(node, graphType));
			}
		}
	}
}
