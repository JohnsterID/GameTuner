using System;
using System.Drawing;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework.Scrollables
{
	public class ScrollableItemTree : IScrollableItem, IScrollableItemTree
	{
		private const int indent_x = 16;

		private const int pad_x = 2;

		private Font font;

		private int height;

		private Rectangle expand = Rectangle.Empty;

		public virtual int ItemHeight
		{
			get
			{
				return height;
			}
		}

		public Font Font
		{
			get
			{
				return font;
			}
		}

		public bool ShowSeparator { get; set; }

		public virtual Image Image { get; set; }

		public object Tag { get; set; }

		public bool Visible { get; set; }

		public virtual string Text { get; set; }

		public ScrollableItemTree(Font font)
		{
			this.font = font;
			ShowSeparator = true;
		}

		public override string ToString()
		{
			return Text;
		}

		public ScrollableItemTree(string text, Font font)
		{
			this.font = font;
			ShowSeparator = true;
			Text = text;
		}

		public virtual void CalcLayout(Graphics g, SizeF size)
		{
			height = Math.Max(19, font.Height);
		}

		public virtual void PaintItem(object sender, ScrollableItemPaintEventArgs e)
		{
			bool flag = e.State == ScrollableItemState.Selected;
			Graphics graphics = e.Graphics;
			Rectangle bounds = e.Bounds;
			if (ShowSeparator)
			{
				graphics.DrawLine(new Pen(Color.FromArgb(186, 182, 169)), bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
			}
			bounds.X = 2 + 16 * e.Level;
			Image styleImage = GetStyleImage(e.Style);
			if (styleImage != null)
			{
				DrawingHelper.DrawImage(graphics, styleImage, bounds.X + 8 - styleImage.Width / 2, bounds.Y + bounds.Height / 2 - styleImage.Height / 2);
				expand.X = bounds.X + 8 - styleImage.Width / 2;
				expand.Y = bounds.Y + bounds.Height / 2 - styleImage.Height / 2;
				expand.Width = 16;
				expand.Height = 16;
			}
			else
			{
				expand = Rectangle.Empty;
			}
			bounds.X += 18;
			if (Image != null)
			{
				DrawingHelper.DrawImage(graphics, Image, bounds.X + 8 - Image.Width / 2, bounds.Y + bounds.Height / 2 - Image.Height / 2);
				bounds.X += 18;
			}
			if (flag)
			{
				graphics.FillRectangle(new SolidBrush(Color.FromArgb(62, 128, 208)), bounds);
			}
			using (StringFormat stringFormat = new StringFormat())
			{
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
				graphics.DrawString(Text, font, flag ? Brushes.White : Brushes.Black, bounds, stringFormat);
			}
		}

		private Image GetStyleImage(ScrollableItemStyle style)
		{
			Image result = null;
			switch (style)
			{
			case ScrollableItemStyle.Expanded:
				result = GameTuner.Framework.Properties.Resources.tl_tree_open;
				break;
			case ScrollableItemStyle.Collapsed:
				result = GameTuner.Framework.Properties.Resources.tl_tree_close;
				break;
			}
			return result;
		}

		public bool HitExpand(int x, int y)
		{
			if (x >= expand.X)
			{
				return x < expand.X + expand.Width;
			}
			return false;
		}
	}
}
