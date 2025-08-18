using System;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class FlowGraphEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public Point2 Location { get; private set; }

		public object Focus { get; private set; }

		public object Target { get; set; }

		public object Original { get; set; }

		public FlowGraphEventArgs()
		{
		}

		public FlowGraphEventArgs(object item, MouseEventArgs e)
		{
			Focus = item;
			Location = new Point2(e.X, e.Y);
		}
	}
}
