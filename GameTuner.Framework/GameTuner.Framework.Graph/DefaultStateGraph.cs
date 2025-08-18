using System;
using System.Collections.Generic;

namespace GameTuner.Framework.Graph
{
	public class DefaultStateGraph : IGraph, IUniqueID, ITagProvider, IServiceProviderProvider, IStateGraph
	{
		public GraphNodeCollection Nodes { get; private set; }

		public int ID { get; set; }

		public object Tag { get; set; }

		public IGraphNode DefaultState { get; set; }

		public IServiceProvider ServiceProvider { get; set; }

		public DefaultStateGraph()
		{
			Nodes = new GraphNodeCollection();
		}

		public IGraphNode CreateNode()
		{
			return new DefaultStateNode(this);
		}

		public void RemoveLinks(IGraphNode node)
		{
			if (node == null)
			{
				return;
			}
			foreach (IStateGraphSocket socket in node.Sockets)
			{
				socket.Disconnect();
			}
			node.Sockets.Clear();
			foreach (IGraphNode node2 in node.Owner.Nodes)
			{
				if (node2 == node)
				{
					continue;
				}
				List<IGraphSocket> remove = new List<IGraphSocket>();
				foreach (IStateGraphSocket socket2 in node2.Sockets)
				{
					if (socket2.Node == node)
					{
						socket2.Disconnect();
						remove.Add((IGraphSocket)socket2);
					}
				}
				node2.Sockets.RemoveAll((IGraphSocket a) => remove.Contains(a));
			}
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
