using System.Windows.Forms;

namespace GameTuner.Framework
{
	public interface INotifyPopupView : IToolTipView
	{
		event MouseEventHandler ClickedNotification;

		void Dismiss();
	}
}
