using System;
using System.Drawing;

namespace GameTuner.Framework
{
	public static class RectangleExtension
	{
		public static bool Inside(this RectangleF rect, Vec2 point)
		{
			if (point.X >= rect.X && point.Y >= rect.Y && point.X <= rect.Right)
			{
				return point.Y <= rect.Bottom;
			}
			return false;
		}

		public static Rectangle ToRect(this RectangleF rect)
		{
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		public static RectangleF SetEdges(this RectangleF rect, float x1, float y1, float x2, float y2)
		{
			rect.X = Math.Min(x1, x2);
			rect.Y = Math.Min(y1, y2);
			rect.Width = Math.Abs(x2 - x1) + 1f;
			rect.Height = Math.Abs(y2 - y1) + 1f;
			return rect;
		}
	}
}
