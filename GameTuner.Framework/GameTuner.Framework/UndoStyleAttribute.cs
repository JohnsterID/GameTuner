using System;

namespace GameTuner.Framework
{
	public class UndoStyleAttribute : Attribute
	{
		public UndoStyle Style { get; private set; }

		public UndoStyleAttribute(UndoStyle style)
		{
			Style = style;
		}
	}
}
