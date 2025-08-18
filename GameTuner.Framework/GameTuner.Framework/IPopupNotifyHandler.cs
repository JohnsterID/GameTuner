using System.Drawing;

namespace GameTuner.Framework
{
	public interface IPopupNotifyHandler
	{
		bool GetNotifyInfo(ref string text, ref Image image, ref string description, object tag);
	}
}
