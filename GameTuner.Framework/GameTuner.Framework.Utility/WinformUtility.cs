using System.Collections.Generic;
using System.Windows.Forms;

namespace GameTuner.Framework.Utility
{
	public static class WinformUtility
	{
		public static IEnumerable<Control> All(this Control.ControlCollection controls)
		{
			foreach (Control control in controls)
			{
				foreach (Control item in control.Controls.All())
				{
					yield return item;
				}
				yield return control;
			}
		}
	}
}
