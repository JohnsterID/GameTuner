using System;

namespace GameTuner.Framework
{
	public class FileExtensionAttribute : Attribute
	{
		public string Extension { get; private set; }

		public bool Primary { get; private set; }

		public FileExtensionAttribute(string ext)
			: this(ext, true)
		{
		}

		public FileExtensionAttribute(string ext, bool primary)
		{
			Extension = ext;
			Primary = primary;
		}
	}
}
