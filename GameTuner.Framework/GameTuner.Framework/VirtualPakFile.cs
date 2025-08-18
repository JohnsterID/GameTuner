using System;

namespace GameTuner.Framework
{
	public class VirtualPakFile : IVirtualItem, IDisposable, IVirtualDirectory
	{
		public VirtualItemCollection Items { get; private set; }

		public string Filename { get; private set; }

		public object Tag { get; set; }

		public IVirtualSpace Owner { get; private set; }

		public string Name
		{
			get
			{
				return Filename;
			}
		}

		public VirtualPakFile(IVirtualSpace owner, string szFilename)
		{
			Owner = owner;
			Filename = szFilename;
			Items = new VirtualItemCollection();
		}

		public void Refresh()
		{
			PackFile packFile = new PackFile(Filename);
			foreach (PackFile.PackFileInfo file in packFile.Files)
			{
				IVirtualItem item = new VirtualPakFileEntry(Owner, file);
				Items.Add(item);
				Owner.OnAddItem(item);
			}
			foreach (IVirtualItem item2 in Items)
			{
				item2.Refresh();
			}
		}

		public void Dispose()
		{
			Clear();
		}

		private void Clear()
		{
			if (Items.Count <= 0)
			{
				return;
			}
			foreach (IVirtualItem item in Items)
			{
				item.Dispose();
			}
			Items.Clear();
		}
	}
}
