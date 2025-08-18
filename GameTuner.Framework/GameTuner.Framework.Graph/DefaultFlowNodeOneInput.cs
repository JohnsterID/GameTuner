using System.ComponentModel;

namespace GameTuner.Framework.Graph
{
	[FlowGraphNodeStyle(FlowGraphNodeStyle.Default)]
	[DisplayName("One Input")]
	internal class DefaultFlowNodeOneInput : DefaultFlowNode
	{
		public DefaultFlowNodeOneInput(IGraph owner)
			: base(owner)
		{
			Add("in", typeof(int_type), FlowGraphSocketType.Input);
		}
	}
}
