using System;

namespace GameTuner.Framework
{
	public class RadioWatcherEventArgs : EventArgs
	{
		public object Item { get; private set; }

		public RadioWatcherEventArgs(object item)
		{
			Item = item;
		}
	}
}
