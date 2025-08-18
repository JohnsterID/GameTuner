using System;
using System.Drawing;
using GameTuner.Framework.Graph;

namespace GameTuner.Framework
{
	public class StateGraphPaintEventArgs : EventArgs
	{
		public IGraphNode Node { get; internal set; }

		public Color SelectColor { get; internal set; }

		public Color TextColor { get; internal set; }

		public Color EdgeColor { get; internal set; }

		public Color DefaultEdgeColor { get; internal set; }

		public Color FillColor { get; internal set; }

		public int Alpha { get; set; }

		public float EdgeWidth { get; internal set; }

		public float LabelWidth { get; internal set; }

		public StringFormat StringFormat { get; internal set; }

		public Graphics Graphics { get; internal set; }

		public Font Font { get; internal set; }

		public Pen Pen { get; internal set; }

		public Brush FillBrush { get; internal set; }

		public Brush TextBrush { get; internal set; }

		public Brush TextLabelBrush { get; internal set; }

		public StateGraphPaintEventArgs(Graphics g, StringFormat sf, Font font, Pen pen, Color sc, Color tc, Color ec, Color dec, Color fc)
		{
			Graphics = g;
			StringFormat = sf;
			Font = font;
			Pen = pen;
			Alpha = 255;
			SelectColor = sc;
			TextColor = tc;
			EdgeColor = ec;
			DefaultEdgeColor = dec;
			FillColor = fc;
		}
	}
}
