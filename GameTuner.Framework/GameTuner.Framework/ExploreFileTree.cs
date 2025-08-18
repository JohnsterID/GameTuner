using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using GameTuner.Framework.Properties;
using GameTuner.Framework.Scrollables;

namespace GameTuner.Framework
{
	public class ExploreFileTree : ScrollableTree, IExploreFileTree
	{
		public class PathContainer : IExpandable
		{
			public DirectoryInfo DirectoryInfo { get; private set; }

			public bool Expanded { get; set; }

			public PathContainer(DirectoryInfo di)
			{
				DirectoryInfo = di;
			}
		}

		public class PackContainer : IExpandable
		{
			public PackFile PackFile { get; private set; }

			public bool Expanded { get; set; }

			public PackContainer(PackFile pf)
			{
				PackFile = pf;
			}
		}

		public interface IFileContainer
		{
			DateTime LastWriteTime { get; }

			long Length { get; }

			string FullName { get; }
		}

		public class FileContainer : IFileContainer
		{
			public FileInfo FileInfo { get; private set; }

			public DateTime LastWriteTime
			{
				get
				{
					return FileInfo.LastWriteTime;
				}
			}

			public long Length
			{
				get
				{
					return FileInfo.Length;
				}
			}

			public string FullName
			{
				get
				{
					return FileInfo.FullName;
				}
			}

			public FileContainer(FileInfo fi)
			{
				FileInfo = fi;
			}
		}

		public class PakFileContainer : IFileContainer
		{
			public PackFile.PackFileInfo PakFileInfo { get; private set; }

			public DateTime LastWriteTime
			{
				get
				{
					return PakFileInfo.Date;
				}
			}

			public long Length
			{
				get
				{
					return PakFileInfo.Size;
				}
			}

			public string FullName
			{
				get
				{
					return Path.Combine(PakFileInfo.Owner.Filename, PakFileInfo.Name);
				}
			}

			public PakFileContainer(PackFile.PackFileInfo kFileInfo)
			{
				PakFileInfo = kFileInfo;
			}
		}

		public delegate IFileContainer FileContainerCreatorHandler(object kFileInfo);

		private string baseDirectory;

		private FileContainerCreatorHandler fileCreator;

		public bool ShowFiles { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public FileContainerCreatorHandler FileContainerCreator
		{
			get
			{
				return fileCreator ?? new FileContainerCreatorHandler(DefaultFileCreator);
			}
			set
			{
				fileCreator = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public FileContainerCreatorHandler PakFileContainerCreator
		{
			get
			{
				return fileCreator ?? new FileContainerCreatorHandler(DefaultFileCreator);
			}
			set
			{
				fileCreator = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string BaseDirectory
		{
			get
			{
				return baseDirectory;
			}
			set
			{
				baseDirectory = value;
				RebuildTree();
			}
		}

		public ExploreFileTree()
		{
			base.ExpandedItemChanged += ExploreFileTree_ExpandedItemChanged;
			base.SelectedItemChanged += ExploreFileTree_SelectedItemChanged;
			base.DisplayFilter = FilterFiles;
		}

		private bool FilterFiles(TreeNode node)
		{
			if (!ShowFiles && !(node.Item.Tag is PathContainer))
			{
				return node.Item.Tag is PackContainer;
			}
			return true;
		}

		private IFileContainer DefaultFileCreator(object kFileInfo)
		{
			if (kFileInfo is PackFile.PackFileInfo)
			{
				return new PakFileContainer(kFileInfo as PackFile.PackFileInfo);
			}
			if (kFileInfo is FileInfo)
			{
				return new FileContainer(kFileInfo as FileInfo);
			}
			return null;
		}

		public void RebuildTree()
		{
			Cursor = Cursors.WaitCursor;
			BeginUpdate();
			Clear();
			PopulateChildren(null, new DirectoryInfo(BaseDirectory), true);
			base.SelectedNodes.Clear();
			EndUpdate();
			Cursor = Cursors.Default;
		}

		private void PopulateChildren(TreeNode root, PackFile kPackFile, bool recurse)
		{
			if (root != null)
			{
				root.Children.Clear();
			}
			foreach (PackFile.PackFileInfo file in kPackFile.Files)
			{
				ScrollableItemTree scrollableItemTree = new ScrollableItemTree(file.Name, Font);
				scrollableItemTree.Tag = FileContainerCreator(file);
				scrollableItemTree.Image = GameTuner.Framework.Properties.Resources.file_document;
				scrollableItemTree.ShowSeparator = false;
				scrollableItemTree.Visible = ShowFiles;
				Add(root, scrollableItemTree);
			}
		}

		private void PopulateChildren(TreeNode root, DirectoryInfo baseDir, bool recurse)
		{
			if (root != null)
			{
				root.Children.Clear();
			}
			DirectoryInfo[] directories;
			try
			{
				directories = baseDir.GetDirectories();
			}
			catch (Exception e)
			{
				ExceptionLogger.Log(e);
				return;
			}
			DirectoryInfo[] array = directories;
			foreach (DirectoryInfo directoryInfo in array)
			{
				ScrollableItemTree scrollableItemTree = new ScrollableItemTree(directoryInfo.Name, Font);
				scrollableItemTree.Tag = new PathContainer(directoryInfo);
				scrollableItemTree.Image = GameTuner.Framework.Properties.Resources.dir_closed;
				scrollableItemTree.ShowSeparator = false;
				TreeNode root2 = Add(root, scrollableItemTree);
				if (recurse)
				{
					PopulateChildren(root2, directoryInfo, false);
				}
			}
			FileInfo[] files = baseDir.GetFiles();
			FileInfo[] array2 = files;
			foreach (FileInfo fileInfo in array2)
			{
				if (string.Compare(fileInfo.Extension, ".fpk", true) == 0)
				{
					PackFile packFile = new PackFile(fileInfo.FullName);
					ScrollableItemTree scrollableItemTree2 = new ScrollableItemTree(fileInfo.Name, Font);
					scrollableItemTree2.Tag = new PackContainer(packFile);
					scrollableItemTree2.Image = GameTuner.Framework.Properties.Resources.dir_closed;
					scrollableItemTree2.ShowSeparator = false;
					TreeNode root3 = Add(root, scrollableItemTree2);
					if (recurse)
					{
						PopulateChildren(root3, packFile, false);
					}
				}
				else
				{
					ScrollableItemTree scrollableItemTree3 = new ScrollableItemTree(fileInfo.Name, Font);
					scrollableItemTree3.Tag = FileContainerCreator(fileInfo);
					scrollableItemTree3.Image = GameTuner.Framework.Properties.Resources.file_document;
					scrollableItemTree3.ShowSeparator = false;
					scrollableItemTree3.Visible = ShowFiles;
					Add(root, scrollableItemTree3);
				}
			}
		}

		private void ExploreFileTree_ExpandedItemChanged(object sender, TreeNodeEventArgs e)
		{
			IExpandable expandable = e.Node.Item.Tag as IExpandable;
			if (expandable != null && expandable.Expanded)
			{
				Cursor = Cursors.WaitCursor;
				BeginUpdate();
				if (expandable.GetType() == typeof(PathContainer))
				{
					PopulateChildren(e.Node, ((PathContainer)expandable).DirectoryInfo, true);
				}
				else if (expandable.GetType() == typeof(PackContainer))
				{
					PopulateChildren(e.Node, ((PackContainer)expandable).PackFile, true);
				}
				EndUpdate();
				Cursor = Cursors.Default;
			}
		}

		private void ExploreFileTree_SelectedItemChanged(object sender, TreeNodeEventArgs e)
		{
			if (base.SelectedNodes.Count > 0)
			{
				TreeNode treeNode = base.SelectedNodes[0];
				PathContainer pathContainer = treeNode.Item.Tag as PathContainer;
				if (pathContainer != null)
				{
					Cursor = Cursors.WaitCursor;
					BeginUpdate();
					PopulateChildren(treeNode, pathContainer.DirectoryInfo, true);
					EndUpdate();
					Cursor = Cursors.Default;
				}
			}
		}
	}
}
