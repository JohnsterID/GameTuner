using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class ExploreFileList : ListView
	{
		private const int Pad = 4;

		private ImageList imageList;

		private IContainer components;

		public event ScrollableTree.TreeNodeEventHandler DoubleClickedItem;

		public ExploreFileList()
		{
			InitializeComponent();
			DoubleBuffered = true;
		}

		public void PopulateList(ScrollableTree.TreeNode node)
		{
			BeginUpdate();
			base.Items.Clear();
			if (node != null)
			{
				foreach (ScrollableTree.TreeNode child in node.Children)
				{
					ExploreFileTree.IFileContainer fileContainer = child.Item.Tag as ExploreFileTree.IFileContainer;
					ListViewItem listViewItem = base.Items.Add(child.Item.ToString(), (fileContainer != null) ? 1 : 0);
					listViewItem.Tag = child;
					if (fileContainer == null)
					{
						continue;
					}
					SourceControlFile sourceControlFile = fileContainer as SourceControlFile;
					if (sourceControlFile != null)
					{
						try
						{
							sourceControlFile.Refresh();
						}
						catch (Exception e)
						{
							ExceptionLogger.Log(e);
						}
					}
					DateTime lastWriteTime = fileContainer.LastWriteTime;
					listViewItem.SubItems.Add(lastWriteTime.ToShortDateString() + " " + lastWriteTime.ToShortTimeString());
					listViewItem.SubItems.Add(Math.Max(1L, (fileContainer.Length + 1024) / 1024) + " KB");
				}
			}
			EndUpdate();
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameTuner.Framework.ExploreFileList));
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			base.SuspendLayout();
			this.imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "dir_closed.png");
			this.imageList.Images.SetKeyName(1, "file_document.png");
			base.FullRowSelect = true;
			base.OwnerDraw = true;
			base.SmallImageList = this.imageList;
			base.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(ExploreFileList_MouseDoubleClick);
			base.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(ExploreFileList_DrawColumnHeader);
			base.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(ExploreFileList_DrawSubItem);
			base.ResumeLayout(false);
		}

		private void ExploreFileList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ScrollableTree.TreeNodeEventHandler doubleClickedItem = this.DoubleClickedItem;
			if (doubleClickedItem != null)
			{
				ScrollableTree.TreeNode node = ((base.SelectedItems.Count > 0) ? ((ScrollableTree.TreeNode)base.SelectedItems[0].Tag) : null);
				doubleClickedItem(this, new ScrollableTree.TreeNodeEventArgs(node));
			}
		}

		private void ExploreFileList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			Graphics graphics = e.Graphics;
			e.DrawBackground();
			bool flag = (e.ItemState & ListViewItemStates.Focused) != 0;
			bool flag2 = e.ColumnIndex == 0;
			if (flag)
			{
				graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags = StringFormatFlags.NoWrap;
			stringFormat.LineAlignment = StringAlignment.Center;
			Rectangle bounds = e.Bounds;
			if (flag2)
			{
				imageList.Draw(graphics, bounds.X + 2, bounds.Y, e.Item.ImageIndex);
				bounds.X += 20;
			}
			Brush brush = (flag ? SystemBrushes.HighlightText : SystemBrushes.ControlText);
			Region clip = graphics.Clip;
			graphics.SetClip(bounds);
			graphics.DrawString(e.SubItem.Text, Font, brush, bounds, stringFormat);
			graphics.SetClip(clip, CombineMode.Replace);
			if (!flag2)
			{
				return;
			}
			ScrollableTree.TreeNode treeNode = (ScrollableTree.TreeNode)e.Item.Tag;
			SourceControlFile sourceControlFile = treeNode.Item.Tag as SourceControlFile;
			if (sourceControlFile == null)
			{
				return;
			}
			float num = e.Bounds.Left + 2;
			float num2 = e.Bounds.Top;
			if (sourceControlFile.Deleted)
			{
				DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.p4_delete, num, num2);
				return;
			}
			if (sourceControlFile.Controlled)
			{
				if (sourceControlFile.HaveLatest)
				{
					DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.p4_sync, num, num2);
				}
				else
				{
					DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.p4_old, num, num2);
				}
			}
			if (sourceControlFile.NewAdd)
			{
				DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.p4_add, num, num2);
			}
			else if (sourceControlFile.Owner)
			{
				DrawingHelper.DrawImage(graphics, GameTuner.Framework.Properties.Resources.p4_checkout, num, num2);
			}
		}

		private void ExploreFileList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}
	}
}
