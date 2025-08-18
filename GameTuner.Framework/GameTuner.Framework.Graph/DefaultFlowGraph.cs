using System;

namespace GameTuner.Framework.Graph
{
	public class DefaultFlowGraph : IGraph, IUniqueID, ITagProvider, IServiceProviderProvider, IFlowGraph
	{
		public GraphNodeCollection Nodes { get; private set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public Factory<IGraphNode> NodeFactory { get; private set; }

		public IServiceProvider ServiceProvider { get; set; }

		public DefaultFlowGraph()
		{
			Nodes = new GraphNodeCollection();
			NodeFactory = new Factory<IGraphNode>();
			NodeFactory.Add(new Maker<DefaultFlowNodeOneOutput>(this));
			NodeFactory.Add(new Maker<DefaultFlowNodePassThru>(this));
			NodeFactory.Add(new Maker<DefaultFlowNodeOneInput>(this));
			NodeFactory.Add(new Maker<DefaultFlowNodeMixed>(this));
		}

		public void RemoveLinks(IGraphNode node)
		{
			IFlowGraphNode flowGraphNode = (IFlowGraphNode)node;
			foreach (IFlowGraphSocket inputSocket in flowGraphNode.InputSockets)
			{
				inputSocket.Disconnect();
			}
		}

		public IGraphNode CreateNode(string type)
		{
			return CreateNode(type, 0);
		}

		public IGraphNode CreateNode(string type, int id)
		{
			IGraphNode graphNode = NodeFactory.Make(type);
			graphNode.ID = id;
			return graphNode;
		}

		public T GetService<T>()
		{
			if (ServiceProvider != null)
			{
				return (T)ServiceProvider.GetService(typeof(T));
			}
			return default(T);
		}
	}
}
