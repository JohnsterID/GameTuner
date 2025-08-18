using System.Drawing;

namespace GameTuner.Framework.Scrollables
{
	public class ScrollableItemText : IScrollableItem
	{
		private const int pad = 2;

		private string caption;

		private string text;

		private object tag;

		private bool visible;

		private Font boldFont;

		private Font font;

		private int capHeight;

		private int height;

		private bool showCaption;

		private bool hardLine;

		public int ItemHeight
		{
			get
			{
				return height;
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

		public Image Image { get; set; }

		public string Caption
		{
			get
			{
				return caption;
			}
			set
			{
				caption = value;
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public virtual bool ShowCaption
		{
			get
			{
				return showCaption;
			}
			set
			{
				showCaption = value;
			}
		}

		public virtual bool ShowHardLine
		{
			get
			{
				return hardLine;
			}
			set
			{
				hardLine = value;
			}
		}

		public ScrollableItemText(string caption, string text, Font font)
		{
			this.caption = caption;
			this.text = text;
			this.font = font;
			visible = true;
			showCaption = true;
			hardLine = false;
			boldFont = new Font(font.FontFamily.Name, font.SizeInPoints, FontStyle.Bold, GraphicsUnit.Point);
		}

		public void CalcLayout(Graphics g, SizeF size)
		{
			SizeF sizeF = g.MeasureString(text, font, size);
			if (showCaption)
			{
				SizeF sizeF2 = g.MeasureString(caption, boldFont);
				capHeight = (int)sizeF2.Height;
				height = (int)(sizeF2.Height + sizeF.Height);
			}
			else
			{
				capHeight = 0;
				height = (int)sizeF.Height;
			}
			height += 4;
		}

		public void PaintItem(object sender, ScrollableItemPaintEventArgs e)
		{
			bool flag = e.State == ScrollableItemState.Selected;
			Graphics graphics = e.Graphics;
			Rectangle bounds = e.Bounds;
			if (flag)
			{
				graphics.FillRectangle(SystemBrushes.Highlight, bounds);
			}
			graphics.DrawLine(hardLine ? SystemPens.Highlight : Pens.LightGray, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			if (Image != null)
			{
				DrawingHelper.DrawImage(graphics, Image, bounds.X, bounds.Y + bounds.Height / 2 - Image.Height / 2);
				bounds.X += 2 + Image.Width;
			}
			if (showCaption)
			{
				bounds.Y += 2;
				bounds.Height = capHeight;
				graphics.DrawString(caption, boldFont, flag ? Brushes.White : Brushes.Black, bounds, stringFormat);
			}
			bounds = e.Bounds;
			if (Image != null)
			{
				bounds.X += 2 + Image.Width;
			}
			bounds.Y += capHeight + 2;
			bounds.Height -= capHeight;
			graphics.DrawString(text, font, flag ? Brushes.White : Brushes.Black, bounds);
		}
	}
}
