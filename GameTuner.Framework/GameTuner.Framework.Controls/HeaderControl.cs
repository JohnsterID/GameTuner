using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class HeaderControl : Control
	{
		private IContainer components;

		protected override CreateParams CreateParams
		{
			get
			{
				SetStyle(ControlStyles.UserPaint, false);
				CreateParams createParams = base.CreateParams;
				createParams.ClassName = "SysHeader32";
				createParams.Style |= 4226;
				return createParams;
			}
		}

		public HeaderControl()
		{
			InitializeComponent();
			NativeMethods.HDITEM item = new NativeMethods.HDITEM
			{
				mask = 3u,
				cchTextMax = 6,
				cxy = 100,
				pszText = "Monkey"
			};
			Header_InsertItem(new HandleRef(this, base.Handle), 4618, 0, ref item);
			item.pszText = "Abc";
			Header_InsertItem(new HandleRef(this, base.Handle), 4618, 1, ref item);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		private static extern int Header_InsertItem(HandleRef hWnd, int msg, int index, ref NativeMethods.HDITEM item);

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.Name = "HeaderControl";
			base.Size = new System.Drawing.Size(207, 24);
			base.ResumeLayout(false);
		}
	}
}
