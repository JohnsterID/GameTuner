using System;
using System.IO;

namespace GameTuner.Framework
{
	public class FileContainerSourceControl : SourceControlFile, ExploreFileTree.IFileContainer
	{
		public FileInfo FileInfo
		{
			get
			{
				return base.File;
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				return base.File.LastWriteTime;
			}
		}

		public long Length
		{
			get
			{
				return base.File.Length;
			}
		}

		public string FullName
		{
			get
			{
				return base.File.FullName;
			}
		}

		public FileContainerSourceControl(FileInfo fi)
			: base(fi, false)
		{
		}
	}
}
