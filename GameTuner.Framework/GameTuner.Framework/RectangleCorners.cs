using System;

namespace GameTuner.Framework
{
	[Flags]
	public enum RectangleCorners
	{
		None = 0,
		TopLeft = 1,
		TopRight = 2,
		BottomLeft = 4,
		BottomRight = 8,
		All = 0xF
	}
}
