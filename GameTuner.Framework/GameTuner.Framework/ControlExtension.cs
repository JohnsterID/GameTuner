using System;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class ControlExtension
	{
		public static bool IsMouseOver(this Control control)
		{
			return control.RectangleToScreen(control.ClientRectangle).Contains(Cursor.Position);
		}

		public static Control Find(this Control.ControlCollection collection, Predicate<Control> match)
		{
			foreach (Control item in collection)
			{
				if (match(item))
				{
					return item;
				}
			}
			return null;
		}
	}
}
