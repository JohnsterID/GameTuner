using System;
using System.Drawing;

namespace GameTuner.Framework
{
	public class ScrollableItemPaintEventArgs : EventArgs
	{
		public bool Interacting { get; set; }

		public ScrollableItemStyle Style { get; set; }

		public ScrollableItemState State { get; set; }

		public Rectangle Bounds { get; set; }

		public Graphics Graphics { get; set; }

		public int Level { get; set; }

		public ScrollableItemPaintEventArgs()
		{
			Bounds = Rectangle.Empty;
			State = ScrollableItemState.Normal;
			Style = ScrollableItemStyle.Normal;
		}

		public ScrollableItemPaintEventArgs(Graphics graphics, Rectangle bounds, ScrollableItemState state)
		{
			Graphics = graphics;
			Bounds = bounds;
			State = state;
			Style = ScrollableItemStyle.Normal;
		}
	}
}
