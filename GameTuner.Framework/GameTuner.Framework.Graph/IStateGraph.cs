namespace GameTuner.Framework.Graph
{
	public interface IStateGraph
	{
		IGraphNode DefaultState { get; set; }

		IGraphNode CreateNode();
	}
}
