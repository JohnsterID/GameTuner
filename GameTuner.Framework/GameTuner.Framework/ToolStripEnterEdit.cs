using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GameTuner.Framework
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripEnterEdit : ToolStripControlHost
	{
		private class EnterEdit : TextBox
		{
			protected override bool IsInputKey(Keys keyData)
			{
				if (keyData == Keys.Return)
				{
					return true;
				}
				return base.IsInputKey(keyData);
			}
		}

		public TextBox TextBox
		{
			get
			{
				return base.Control as TextBox;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(60, 16);
			}
		}

		public ToolStripEnterEdit()
			: base(CreateControlInstance())
		{
		}

		private static Control CreateControlInstance()
		{
			EnterEdit enterEdit = new EnterEdit();
			enterEdit.AutoSize = false;
			enterEdit.Height = 16;
			enterEdit.Width = 60;
			return enterEdit;
		}
	}
}
