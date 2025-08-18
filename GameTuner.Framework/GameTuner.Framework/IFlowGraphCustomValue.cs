using System.Drawing;

namespace GameTuner.Framework
{
	public interface IFlowGraphCustomValue
	{
		void PaintCustomValue(Graphics g, RectangleF bounds, Font font, StringFormat sf);
	}
}
