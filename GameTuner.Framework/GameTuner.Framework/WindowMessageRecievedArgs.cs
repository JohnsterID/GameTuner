using System;

namespace GameTuner.Framework
{
	public class WindowMessageRecievedArgs : EventArgs
	{
		public IntPtr Sender;

		public string Message;
	}
}
