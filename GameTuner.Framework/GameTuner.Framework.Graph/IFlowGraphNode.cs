using System;

namespace GameTuner.Framework.Graph
{
	public interface IFlowGraphNode
	{
		GraphSocketCollection InputSockets { get; }

		GraphSocketCollection OutputSockets { get; }

		bool IsOfType(IGraphSocket socket, FlowGraphSocketType type);

		IGraphSocket Add(string name, Type socketType, FlowGraphSocketType type);

		IGraphSocket Add(string name, Type socketType, FlowGraphSocketType type, int id);

		IGraphSocket Find(string name);
	}
}
