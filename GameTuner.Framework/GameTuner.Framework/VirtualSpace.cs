using System;
using System.Collections.Generic;
using System.IO;

namespace GameTuner.Framework
{
	public class VirtualSpace : IVirtualSpace, IDisposable
	{
		private IList<string> basePaths = new List<string>();

		private Dictionary<string, IVirtualItem> fileDict;

		public IList<string> BasePaths
		{
			get
			{
				return basePaths;
			}
			set
			{
				basePaths = value;
			}
		}

		public IVirtualItem Root { get; private set; }

		public ICollection<IVirtualItem> Items
		{
			get
			{
				return fileDict.Values;
			}
		}

		public ICollection<IVirtualDirectory> Directories { get; private set; }

		public VirtualSpace()
		{
			fileDict = new Dictionary<string, IVirtualItem>();
			Directories = new List<IVirtualDirectory>();
		}

		~VirtualSpace()
		{
			Clear();
		}

		public void Dispose()
		{
			Clear();
		}

		private void Clear()
		{
			if (Root != null)
			{
				Root.Dispose();
				Root = null;
			}
			fileDict.Clear();
		}

		public void Refresh()
		{
			Clear();
			VirtualContainer virtualContainer = (VirtualContainer)(Root = new VirtualContainer(this));
			foreach (string basePath in BasePaths)
			{
				if (!string.IsNullOrEmpty(basePath))
				{
					try
					{
						virtualContainer.Items.Add(new VirtualDirectory(this, new DirectoryInfo(basePath)));
					}
					catch (Exception e)
					{
						Clear();
						ExceptionLogger.Log(e, "Scanning virtual space");
					}
				}
			}
			virtualContainer.Refresh();
		}

		public void OnAddItem(IVirtualItem item)
		{
			IVirtualFile virtualFile = item as IVirtualFile;
			if (virtualFile != null)
			{
				string key = virtualFile.Name.ToLower();
				IVirtualItem value;
				if (!fileDict.TryGetValue(key, out value))
				{
					fileDict.Add(key, item);
				}
				else if (value != item)
				{
					ExceptionLogger.Log("Warning: Duplicate File", string.Format("<color name=\"FireBrick\"><b>Duplicate File name detected:</b></color><br>{0}<br>{1}", virtualFile.FullPath, ((IVirtualFile)fileDict[key]).FullPath), ValidationResultLevel.Warning);
				}
			}
			else
			{
				IVirtualDirectory virtualDirectory = item as IVirtualDirectory;
				if (virtualDirectory != null)
				{
					Directories.Add(virtualDirectory);
				}
			}
		}

		public bool FileExists(string file)
		{
			return fileDict.ContainsKey(Path.GetFileName(file).ToLower());
		}

		public string FindFullPath(string file)
		{
			IVirtualItem value;
			if (fileDict.TryGetValue(Path.GetFileName(file).ToLower(), out value))
			{
				return ((IVirtualFile)value).FullPath;
			}
			if (Path.IsPathRooted(file))
			{
				return file;
			}
			return null;
		}

		public IVirtualItem FindItem(string name)
		{
			string directoryName = Path.GetDirectoryName(name);
			if (directoryName.Length > 0)
			{
				IVirtualDirectory virtualDirectory = FindDirectoryByName(Root, directoryName);
				if (virtualDirectory == null)
				{
					return null;
				}
				return FindFileByName(virtualDirectory as IVirtualItem, Path.GetFileName(name)) as IVirtualItem;
			}
			return FindFileByName(Root, Path.GetFileName(name)) as IVirtualItem;
		}

		public IVirtualDirectory FindDirectoryByName(IVirtualItem item, string szDirectory)
		{
			foreach (IVirtualDirectory directory in Directories)
			{
				if (directory != null && string.Compare(szDirectory, directory.Name, true) == 0)
				{
					return directory;
				}
			}
			return null;
		}

		private IVirtualFile FindFileByName(IVirtualItem item, string name)
		{
			IVirtualDirectory virtualDirectory = item as IVirtualDirectory;
			if (virtualDirectory != null)
			{
				foreach (IVirtualItem item2 in virtualDirectory.Items)
				{
					IVirtualFile virtualFile = item2 as IVirtualFile;
					if (virtualFile != null && string.Compare(name, virtualFile.Name, true) == 0)
					{
						return virtualFile;
					}
					IVirtualDirectory virtualDirectory2 = item2 as IVirtualDirectory;
					if (virtualDirectory2 != null)
					{
						IVirtualFile virtualFile2 = FindFileByName(item2, name);
						if (virtualFile2 != null)
						{
							return virtualFile2;
						}
					}
				}
			}
			return null;
		}
	}
}
