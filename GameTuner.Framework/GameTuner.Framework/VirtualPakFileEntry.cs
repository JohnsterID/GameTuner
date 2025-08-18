using System;
using System.IO;

namespace GameTuner.Framework
{
	public class VirtualPakFileEntry : IVirtualItem, IDisposable, IVirtualFile
	{
		public IVirtualSpace Owner { get; private set; }

		public PackFile.PackFileInfo FileInfo { get; private set; }

		public object Tag { get; set; }

		public string FullPath
		{
			get
			{
				if (FileInfo.Owner.Filename != null)
				{
					return FileInfo.Owner.Filename + "\\" + Name;
				}
				return Name;
			}
		}

		public string Name
		{
			get
			{
				return FileInfo.Name;
			}
		}

		public Stream Stream
		{
			get
			{
				return FileInfo.Owner.GetFileStream(FileInfo);
			}
		}

		public VirtualPakFileEntry(IVirtualSpace owner, PackFile.PackFileInfo kPFI)
		{
			Owner = owner;
			FileInfo = kPFI;
		}

		public void Refresh()
		{
		}

		public void Dispose()
		{
		}
	}
}
