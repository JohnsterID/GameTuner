using System.ComponentModel;

namespace GameTuner.Framework.Graph
{
	[DisplayName("One Output")]
	[ColorProvider(170, 214, 119)]
	[FlowGraphNodeStyle(FlowGraphNodeStyle.Constant)]
	internal class DefaultFlowNodeOneOutput : DefaultFlowNode
	{
		public DefaultFlowNodeOneOutput(IGraph owner)
			: base(owner)
		{
			Add("out", typeof(int_type), FlowGraphSocketType.Output);
		}
	}
}
