using System.ComponentModel;

namespace GameTuner.Framework.Graph
{
	[DisplayName("Pass thru")]
	internal class DefaultFlowNodePassThru : DefaultFlowNode
	{
		public DefaultFlowNodePassThru(IGraph owner)
			: base(owner)
		{
			Add("out", typeof(int_type), FlowGraphSocketType.Output);
			Add("input", typeof(int_type), FlowGraphSocketType.Input);
		}
	}
}
