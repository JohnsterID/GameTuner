using System;
using System.IO;

namespace GameTuner.Framework
{
	internal class VirtualDirectory : IVirtualItem, IDisposable, IVirtualDirectory
	{
		public object Tag { get; set; }

		public IVirtualSpace Owner { get; private set; }

		public VirtualItemCollection Items { get; private set; }

		public DirectoryInfo Directory { get; private set; }

		public string Name
		{
			get
			{
				return Directory.FullName;
			}
		}

		public VirtualDirectory(IVirtualSpace owner, DirectoryInfo di)
		{
			Owner = owner;
			Directory = di;
			Items = new VirtualItemCollection();
		}

		public virtual void Refresh()
		{
			Clear();
			DirectoryInfo[] directories = Directory.GetDirectories();
			DirectoryInfo[] array = directories;
			foreach (DirectoryInfo di in array)
			{
				IVirtualItem item = new VirtualDirectory(Owner, di);
				Items.Add(item);
				Owner.OnAddItem(item);
			}
			FileInfo[] files = Directory.GetFiles();
			FileInfo[] array2 = files;
			foreach (FileInfo fileInfo in array2)
			{
				if (Path.GetExtension(fileInfo.FullName) == ".fpk" && PackFile.IsPackFile(fileInfo.FullName))
				{
					IVirtualItem item2 = new VirtualPakFile(Owner, fileInfo.FullName);
					Items.Add(item2);
					Owner.OnAddItem(item2);
				}
				else
				{
					IVirtualItem item3 = new VirtualFile(Owner, fileInfo);
					Items.Add(item3);
					Owner.OnAddItem(item3);
				}
			}
			foreach (IVirtualItem item4 in Items)
			{
				item4.Refresh();
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
