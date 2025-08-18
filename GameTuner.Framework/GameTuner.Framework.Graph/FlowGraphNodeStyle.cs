using System;

namespace GameTuner.Framework.Graph
{
	[Flags]
	public enum FlowGraphNodeStyle
	{
		None = 0,
		NoDelete = 1,
		NoCreate = 2,
		Constant = 4,
		Default = 8,
		HideOutput = 0x20,
		HideInput = 0x40
	}
}
