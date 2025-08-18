using System;

namespace GameTuner.Framework.Graph
{
	public class FlowGraphNodeStyleAttribute : Attribute
	{
		public FlowGraphNodeStyle Style { get; private set; }

		public FlowGraphNodeStyleAttribute()
		{
			Style = FlowGraphNodeStyle.None;
		}

		public FlowGraphNodeStyleAttribute(FlowGraphNodeStyle style)
		{
			Style = style;
		}
	}
}
