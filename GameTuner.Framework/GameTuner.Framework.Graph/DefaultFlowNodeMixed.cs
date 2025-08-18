using System.ComponentModel;

namespace GameTuner.Framework.Graph
{
	[ColorProvider(131, 170, 233)]
	[DisplayName("Mixed")]
	internal class DefaultFlowNodeMixed : DefaultFlowNode
	{
		public DefaultFlowNodeMixed(IGraph owner)
			: base(owner)
		{
			Add("iout", typeof(int_type), FlowGraphSocketType.Output);
			Add("fout", typeof(float_type), FlowGraphSocketType.Output);
			Add("iin", typeof(int_type), FlowGraphSocketType.Input);
			Add("fin", typeof(float_type), FlowGraphSocketType.Input);
		}
	}
}
