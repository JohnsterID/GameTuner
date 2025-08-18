using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Resources;

namespace GameTuner.Framework
{
	[ToolboxBitmap(typeof(ResourceTag), "splitter.bmp")]
	[Description("SplitContainer implementation that does not leave any pixel droppings on focus")]
	public class SplitterControl : SplitContainer
	{
		protected override void OnPaint(PaintEventArgs e)
		{
		}
	}
}
