using System.Drawing;
using System.Drawing.Drawing2D;

namespace GameTuner.Framework.Scrollables
{
	public class ScrollableItemImage : IScrollableItem
	{
		private const int ImageHeight = 32;

		private const int ItemPad = 5;

		private Image image;

		private Font font;

		private object tag;

		private bool visible;

		private string caption;

		public int ItemHeight
		{
			get
			{
				return 42;
			}
		}

		public object Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}

		public ScrollableItemImage(string caption, Image image, Font font)
		{
			this.caption = caption;
			this.image = image;
			this.font = font;
			visible = true;
		}

		public void CalcLayout(Graphics g, SizeF size)
		{
		}

		public void PaintItem(object sender, ScrollableItemPaintEventArgs e)
		{
			bool flag = e.State == ScrollableItemState.Selected;
			Graphics graphics = e.Graphics;
			Rectangle bounds = e.Bounds;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Far;
			stringFormat.LineAlignment = StringAlignment.Center;
			Pen pen = new Pen(Color.FromArgb(79, 79, 79));
			pen.DashStyle = DashStyle.Dot;
			graphics.DrawLine(pen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
			if (flag)
			{
				graphics.FillRectangle(new SolidBrush(Color.FromArgb(51, 153, 255)), bounds);
			}
			if (image != null)
			{
				Rectangle empty = Rectangle.Empty;
				empty.X = 5;
				empty.Y = bounds.Y + ItemHeight / 2 - 16;
				empty.Width = 32;
				empty.Height = 32;
				DrawingHelper.DrawChecker(graphics, empty);
				Rectangle rect = empty;
				rect.Width = (int)(32f * ((float)image.Width * 1f / (float)image.Height));
				rect.Height = 32;
				rect.X = 21 - rect.Width / 2;
				graphics.DrawImage(image, rect);
				string s = string.Format("{0}\n({1} x {2})", caption, image.Width, image.Height);
				graphics.DrawString(s, font, flag ? Brushes.White : Brushes.Black, bounds, stringFormat);
			}
		}
	}
}
