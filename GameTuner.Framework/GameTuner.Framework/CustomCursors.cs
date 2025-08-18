using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public sealed class CustomCursors
	{
		private Dictionary<CustomCursor, Cursor> cursors;

		private static CustomCursors self;

		private CustomCursors()
		{
			cursors = new Dictionary<CustomCursor, Cursor>();
		}

		public Cursor GetCursor(CustomCursor cursor)
		{
			Cursor value = null;
			if (!cursors.TryGetValue(cursor, out value))
			{
				byte[] buffer = null;
				switch (cursor)
				{
				case CustomCursor.FingerPoint:
					buffer = GameTuner.Framework.Properties.Resources.fpoint;
					break;
				case CustomCursor.HandDrag:
					buffer = GameTuner.Framework.Properties.Resources.hand;
					break;
				case CustomCursor.LeftExtend:
					buffer = GameTuner.Framework.Properties.Resources.left_extend;
					break;
				case CustomCursor.RightExtend:
					buffer = GameTuner.Framework.Properties.Resources.right_extend;
					break;
				case CustomCursor.HSplit:
					buffer = GameTuner.Framework.Properties.Resources.splith;
					break;
				case CustomCursor.VSplit:
					buffer = GameTuner.Framework.Properties.Resources.splitv;
					break;
				}
				value = new Cursor(new MemoryStream(buffer));
				cursors.Add(cursor, value);
			}
			return value;
		}

		public static Cursor Get(CustomCursor cursor)
		{
			if (self == null)
			{
				self = new CustomCursors();
			}
			return self.GetCursor(cursor);
		}

		public static void FreeCursors()
		{
			if (self != null)
			{
				self.cursors.Clear();
			}
		}
	}
}
