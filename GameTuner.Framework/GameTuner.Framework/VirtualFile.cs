using System;
using System.IO;

namespace GameTuner.Framework
{
	public class VirtualFile : IVirtualItem, IDisposable, IVirtualFile
	{
		public object Tag { get; set; }

		public IVirtualSpace Owner { get; private set; }

		public FileInfo FileInfo { get; private set; }

		public string FullPath
		{
			get
			{
				return FileInfo.FullName;
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
				return new FileStream(FullPath, FileMode.Open, FileAccess.Read);
			}
		}

		public VirtualFile(IVirtualSpace owner, FileInfo fi)
		{
			Owner = owner;
			FileInfo = fi;
		}

		public virtual void Refresh()
		{
		}

		public void Dispose()
		{
		}
	}
}
