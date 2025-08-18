using System;

namespace GameTuner.Framework.Graph
{
	public interface IFlowGraphSocket
	{
		string Name { get; set; }

		IGraphNode Node { get; }

		IGraphSocket OutputSocket { get; }

		FlowGraphSocketType FlowType { get; }

		Type SocketType { get; }

		void Connect(IGraphNode node, IGraphSocket socket);

		void Disconnect();
	}
}
