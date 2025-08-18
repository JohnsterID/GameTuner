namespace GameTuner.Framework.Graph
{
	public interface IFlowGraph
	{
		Factory<IGraphNode> NodeFactory { get; }

		IGraphNode CreateNode(string type);

		IGraphNode CreateNode(string type, int id);
	}
}
