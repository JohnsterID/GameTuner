using System;

namespace GameTuner.Framework
{
	[Flags]
	public enum UndoStyle
	{
		None = 0,
		Property = 1,
		Delete = 2,
		Create = 4,
		State = 8
	}
}
