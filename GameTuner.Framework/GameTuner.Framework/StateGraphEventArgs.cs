using System;
using System.Windows.Forms;
using GameTuner.Framework.Graph;

namespace GameTuner.Framework
{
	public class StateGraphEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public Point2 Location { get; private set; }

		public object Focus { get; private set; }

		public IGraphNode NodeA { get; private set; }

		public IGraphNode NodeB { get; private set; }

		public StateGraphEventArgs()
		{
		}

		public StateGraphEventArgs(object item, MouseEventArgs e)
		{
			Focus = item;
			Location = new Point2(e.X, e.Y);
		}

		public StateGraphEventArgs(IGraphNode nodeA, IGraphNode nodeB)
		{
			NodeA = nodeA;
			NodeB = nodeB;
			Location = Point2.Empty;
		}
	}
}
