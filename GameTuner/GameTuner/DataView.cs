using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameTuner
{
	public class DataView : Panel, ICustomControl
	{
		private TreeView m_TreeView = new TreeView();

		private int m_iNetworkListener;

		private ContextMenuStrip m_mnuContext = new ContextMenuStrip();

		private ToolStripMenuItem m_btnMove = new ToolStripMenuItem("Move");

		private ToolStripMenuItem m_btnResize = new ToolStripMenuItem("Resize");

		private ToolStripMenuItem m_btnDelete = new ToolStripMenuItem("Delete");

		public DataView(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
		{
			base.Width = 530;
			base.Height = 400;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.MouseMove += onMouseMove;
			base.MouseClick += onMouseClick;
			m_TreeView.Sorted = false;
			m_TreeView.MouseClick += onMouseClick;
			m_TreeView.MouseMove += onMouseMove;
			m_TreeView.BeforeExpand += m_TreeView_BeforeExpand;
			m_TreeView.AfterCollapse += m_TreeView_AfterCollapse;
			m_TreeView.Top = 0;
			m_TreeView.Left = 0;
			m_TreeView.Height = base.Height - m_TreeView.Top;
			m_TreeView.Width = base.Width;
			m_TreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			base.Controls.Add(m_TreeView);
			m_btnMove.Click += m_btnMove_Click;
			m_btnResize.Click += m_btnResize_Click;
			m_btnDelete.Click += m_btnDelete_Click;
			m_mnuContext.Items.Add(m_btnMove);
			m_mnuContext.Items.Add(m_btnResize);
			m_mnuContext.Items.Add(new ToolStripSeparator());
			m_mnuContext.Items.Add(m_btnDelete);
			ContextMenuStrip = m_mnuContext;
			m_iNetworkListener = Connection.Instance.AddRequestListener(CompletedAction);
			MakeNodeInfoRequest(-1);
		}

		public void Release()
		{
			Connection.Instance.RemoveRequestListener(m_iNetworkListener);
		}

		public void LuaStateChanged(LuaState state, LuaState lastState)
		{
		}

		private TreeNode FindNodeByID(int iID, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.Tag != null && (int)node.Tag == iID)
				{
					return node;
				}
			}
			foreach (TreeNode node2 in nodes)
			{
				TreeNode treeNode3 = FindNodeByID(iID, node2.Nodes);
				if (treeNode3 != null)
				{
					return treeNode3;
				}
			}
			return null;
		}

		public void CompletedAction(List<string> luaMessages)
		{
			int result;
			if (luaMessages.Count == 0 || !int.TryParse(luaMessages[0], out result))
			{
				return;
			}
			TreeNodeCollection treeNodeCollection = null;
			if (result < 0)
			{
				treeNodeCollection = m_TreeView.Nodes;
			}
			else
			{
				TreeNode treeNode = FindNodeByID(result, m_TreeView.Nodes);
				if (treeNode != null)
				{
					treeNodeCollection = treeNode.Nodes;
				}
			}
			if (treeNodeCollection == null)
			{
				return;
			}
			m_TreeView.BeginUpdate();
			treeNodeCollection.Clear();
			TreeNode treeNode2 = null;
			if (result > 0)
			{
				treeNode2 = treeNodeCollection.Add("Data Members");
			}
			for (int i = 1; i < luaMessages.Count; i++)
			{
				string text = luaMessages[i];
				if (text[0] == 'D')
				{
					treeNode2.Nodes.Add(text.Substring(1));
				}
				else if (text[0] == 'N')
				{
					int num = text.IndexOf('|');
					int result2;
					if (num > 1 && int.TryParse(text.Substring(1, num - 1), out result2))
					{
						TreeNode treeNode3 = treeNodeCollection.Add(text.Substring(num + 1));
						treeNode3.Tag = result2;
						treeNode3.Nodes.Add("Populating List...");
					}
				}
			}
			m_TreeView.EndUpdate();
		}

		public void StartDrag()
		{
			base.BorderStyle = BorderStyle.Fixed3D;
			m_TreeView.Visible = false;
		}

		public void EndDrag()
		{
			base.BorderStyle = BorderStyle.FixedSingle;
			m_TreeView.Visible = true;
		}

		public void TabEntered()
		{
		}

		public void TabLeft()
		{
		}

		private void MakeNodeInfoRequest(int iID)
		{
			Connection.Instance.Request("TREE:" + iID, m_iNetworkListener);
		}

		private void m_btnMove_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.MovingControl = this;
		}

		private void m_btnResize_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.ResizingControl = this;
		}

		private void m_btnDelete_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this control?", "Deleting Control", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
			if (dialogResult == DialogResult.Yes)
			{
				CustomUI customUI = (CustomUI)base.Parent;
				customUI.Controls.Remove(this);
				customUI.Dirty = true;
			}
		}

		private void m_TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			TreeNode node = e.Node;
			if (node != null && node.Tag != null)
			{
				MakeNodeInfoRequest((int)node.Tag);
			}
		}

		private void m_TreeView_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			TreeNode node = e.Node;
			if (node != null && node.Tag != null)
			{
				e.Node.Nodes.Clear();
				e.Node.Nodes.Add("Populating List...");
			}
		}
	}
}
