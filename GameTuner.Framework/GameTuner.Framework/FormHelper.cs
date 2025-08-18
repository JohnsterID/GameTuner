using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class FormHelper
	{
		public static void EnsureVisible(Form form)
		{
			form.Show();
			form.Activate();
			form.WindowState = FormWindowState.Normal;
			Screen[] allScreens = Screen.AllScreens;
			foreach (Screen screen in allScreens)
			{
				if (screen.Bounds.IntersectsWith(form.DesktopBounds))
				{
					return;
				}
			}
			Screen primaryScreen = Screen.PrimaryScreen;
			form.DesktopLocation = primaryScreen.Bounds.Location;
		}
	}
}
