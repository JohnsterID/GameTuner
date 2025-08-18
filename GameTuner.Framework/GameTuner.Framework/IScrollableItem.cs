using System.Drawing;

namespace GameTuner.Framework
{
	public interface IScrollableItem
	{
		int ItemHeight { get; }

		object Tag { get; set; }

		bool Visible { get; set; }

		void CalcLayout(Graphics g, SizeF size);

		void PaintItem(object sender, ScrollableItemPaintEventArgs e);
	}
}
