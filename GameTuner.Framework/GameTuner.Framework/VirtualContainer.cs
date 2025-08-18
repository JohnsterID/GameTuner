using System;

namespace GameTuner.Framework
{
	public class VirtualContainer : IVirtualItem, IDisposable, IVirtualDirectory
	{
		public object Tag { get; set; }

		public IVirtualSpace Owner { get; set; }

		public string Name { get; set; }

		public VirtualItemCollection Items { get; set; }

		public VirtualContainer(IVirtualSpace kOwner)
		{
			Owner = kOwner;
			Items = new VirtualItemCollection();
		}

		public void Refresh()
		{
			foreach (IVirtualItem item in Items)
			{
				item.Refresh();
			}
		}

		public void Dispose()
		{
			Items.Clear();
		}
	}
}
