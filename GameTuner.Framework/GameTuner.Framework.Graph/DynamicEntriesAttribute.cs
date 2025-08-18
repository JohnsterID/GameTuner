using System;

namespace GameTuner.Framework.Graph
{
	public class DynamicEntriesAttribute : Attribute
	{
		public DynamicEntriesStyle Style { get; private set; }

		public int StartIndex { get; private set; }

		public DynamicEntriesAttribute(DynamicEntriesStyle style)
			: this(style, 0)
		{
		}

		public DynamicEntriesAttribute(DynamicEntriesStyle style, int startIndex)
		{
			Style = style;
			StartIndex = startIndex;
		}
	}
}
