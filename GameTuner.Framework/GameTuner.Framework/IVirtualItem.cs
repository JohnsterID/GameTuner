using System;

namespace GameTuner.Framework
{
	public interface IVirtualItem : IDisposable
	{
		object Tag { get; set; }

		IVirtualSpace Owner { get; }

		void Refresh();
	}
}
