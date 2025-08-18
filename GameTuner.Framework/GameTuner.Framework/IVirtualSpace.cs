using System;
using System.Collections.Generic;

namespace GameTuner.Framework
{
	public interface IVirtualSpace : IDisposable
	{
		IList<string> BasePaths { get; set; }

		IVirtualItem Root { get; }

		ICollection<IVirtualItem> Items { get; }

		ICollection<IVirtualDirectory> Directories { get; }

		string FindFullPath(string file);

		bool FileExists(string file);

		IVirtualItem FindItem(string name);

		void Refresh();

		void OnAddItem(IVirtualItem item);
	}
}
