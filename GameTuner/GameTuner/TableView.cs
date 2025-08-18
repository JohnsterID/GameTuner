using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameTuner.Framework;

namespace GameTuner
{
	public class TableView : Panel, ICustomControl
	{
		public string OnRefresh = string.Empty;

		private TextBox m_txtTable = new TextBox();

		private TreeView m_TableDisp = new TreeView();

		private Button m_btnRefresh = new Button();

		private Timer m_UpdateTableTimer = new Timer();

		private AutoCompletePopup m_CodeBuddyWnd = new AutoCompletePopup();

		private int m_iNetworkListener;

		private int m_iHelpNetworkListener;

		private int m_iRefreshActionListener;

		private ContextMenuStrip m_mnuContext = new ContextMenuStrip();

		private ToolStripMenuItem m_btnEdit = new ToolStripMenuItem("Edit");

		private ToolStripMenuItem m_btnMove = new ToolStripMenuItem("Move");

		private ToolStripMenuItem m_btnResize = new ToolStripMenuItem("Resize");

		private ToolStripMenuItem m_btnDelete = new ToolStripMenuItem("Delete");

		private string m_sPrevInputText = string.Empty;

		private static char[] m_CodeBuddyTriggers = new char[4] { ':', '.', '[', ']' };

		public string Table
		{
			get
			{
				return m_txtTable.Text;
			}
			set
			{
				m_txtTable.Text = value;
			}
		}

		public bool Fixed
		{
			get
			{
				return m_txtTable.ReadOnly;
			}
			set
			{
				m_txtTable.ReadOnly = value;
			}
		}

		public TableView(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
		{
			base.Width = 530;
			base.Height = 400;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.MouseMove += onMouseMove;
			base.MouseClick += onMouseClick;
			m_CodeBuddyWnd.Owner = frmMainForm.MainForm;
			m_CodeBuddyWnd.TextBox = m_txtTable;
			m_CodeBuddyWnd.OnSelection += OnCodeBuddySelection;
			m_CodeBuddyWnd.FindReplacementSymbol = FindLastSymbol;
			m_btnRefresh.Text = "Refresh";
			m_btnRefresh.Click += m_btnRefresh_Click;
			m_btnRefresh.BackColor = SystemColors.Control;
			m_btnRefresh.ForeColor = SystemColors.ControlText;
			m_btnRefresh.Width = (int)m_btnRefresh.CreateGraphics().MeasureString(m_btnRefresh.Text, m_btnRefresh.Font).Width + 10;
			m_btnRefresh.Location = new Point(base.Width - m_btnRefresh.Width, 0);
			m_btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			base.Controls.Add(m_btnRefresh);
			m_txtTable.MouseClick += onMouseClick;
			m_txtTable.MouseMove += onMouseMove;
			m_txtTable.KeyDown += m_txtTable_KeyDown;
			m_txtTable.TextChanged += m_txtTable_TextChanged;
			m_txtTable.Location = new Point(0, 0);
			m_txtTable.Width = base.Width - m_btnRefresh.Width;
			m_txtTable.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			base.Controls.Add(m_txtTable);
			frmMainForm.MainForm.ControlAcceptsTab(m_txtTable);
			m_TableDisp.Sorted = true;
			m_TableDisp.MouseClick += onMouseClick;
			m_TableDisp.MouseMove += onMouseMove;
			m_TableDisp.BeforeExpand += m_TableDisp_BeforeExpand;
			m_TableDisp.AfterCollapse += m_TableDisp_AfterCollapse;
			m_TableDisp.Top = m_txtTable.Height + 2;
			m_TableDisp.Left = 0;
			m_TableDisp.Height = base.Height - m_TableDisp.Top;
			m_TableDisp.Width = base.Width;
			m_TableDisp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			base.Controls.Add(m_TableDisp);
			m_btnEdit.Click += m_btnEdit_Click;
			m_btnMove.Click += m_btnMove_Click;
			m_btnResize.Click += m_btnResize_Click;
			m_btnDelete.Click += m_btnDelete_Click;
			m_mnuContext.Items.Add(m_btnEdit);
			m_mnuContext.Items.Add(m_btnMove);
			m_mnuContext.Items.Add(m_btnResize);
			m_mnuContext.Items.Add(new ToolStripSeparator());
			m_mnuContext.Items.Add(m_btnDelete);
			ContextMenuStrip = m_mnuContext;
			m_UpdateTableTimer.Interval = 250;
			m_UpdateTableTimer.Tick += m_UpdateTableTimer_Tick;
			m_iNetworkListener = Connection.Instance.AddRequestListener(CompletedAction);
			m_iHelpNetworkListener = Connection.Instance.AddRequestListener(OnHelpRecieved);
			m_iRefreshActionListener = Connection.Instance.AddRequestListener(CompletedRefreshAction);
		}

		public void Release()
		{
			Connection.Instance.RemoveRequestListener(m_iNetworkListener);
			Connection.Instance.RemoveRequestListener(m_iHelpNetworkListener);
			Connection.Instance.RemoveRequestListener(m_iRefreshActionListener);
		}

		public void LuaStateChanged(LuaState state, LuaState lastState)
		{
			RefreshTableDisp();
		}

		public void CompletedRefreshAction(List<string> messages)
		{
		}

		public void CompletedAction(List<string> luaMessages)
		{
			if (luaMessages.Count == 0 || !luaMessages[0].Contains(Table))
			{
				m_TableDisp.Nodes.Clear();
				return;
			}
			m_TableDisp.BeginUpdate();
			TreeNodeCollection treeNodeCollection = null;
			treeNodeCollection = m_TableDisp.Nodes;
			string text = luaMessages[0].Substring(Table.Length);
			string[] array = text.Split('.', '[', ']');
			int num = ((!(Table == string.Empty)) ? 1 : 0);
			for (int i = num; i < array.Length; i++)
			{
				if (treeNodeCollection == null)
				{
					break;
				}
				if (array[i] != string.Empty)
				{
					TreeNode[] array2 = treeNodeCollection.Find(array[i], false);
					treeNodeCollection = ((array2.Length <= 0) ? null : array2[0].Nodes);
				}
			}
			if (treeNodeCollection != null)
			{
				treeNodeCollection.Clear();
				for (int j = 1; j < luaMessages.Count; j++)
				{
					string text2 = luaMessages[j];
					if (text2.Contains("="))
					{
						treeNodeCollection.Add(text2);
						continue;
					}
					TreeNode treeNode = treeNodeCollection.Add(text2, text2);
					treeNode.Nodes.Add("Populating List...");
				}
			}
			m_TableDisp.EndUpdate();
		}

		public void StartDrag()
		{
			base.BorderStyle = BorderStyle.Fixed3D;
			m_btnRefresh.Visible = false;
			m_txtTable.Visible = false;
			m_TableDisp.Visible = false;
		}

		public void EndDrag()
		{
			base.BorderStyle = BorderStyle.FixedSingle;
			m_btnRefresh.Visible = true;
			m_txtTable.Visible = true;
			m_TableDisp.Visible = true;
		}

		public void TabEntered()
		{
		}

		public void TabLeft()
		{
		}

		public void RefreshTableDisp()
		{
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null && customUI.LuaState != null)
			{
				if (!string.IsNullOrEmpty(OnRefresh))
				{
					Connection.Instance.Request("CMD:" + customUI.LuaState.ID + ":" + OnRefresh, m_iRefreshActionListener);
				}
				MakeTableRequest(Table);
			}
		}

		private void MakeTableRequest(string sTable)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null && customUI.LuaState != null)
			{
				Connection.Instance.Request("TABLE:" + customUI.LuaState.ID + ":" + sTable, m_iNetworkListener);
			}
		}

		private void m_btnEdit_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null)
			{
				customUI.EditTableView(this);
			}
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

		private void m_btnRefresh_Click(object sender, EventArgs e)
		{
			RefreshTableDisp();
		}

		private void m_txtTable_KeyDown(object sender, KeyEventArgs e)
		{
			if (!m_CodeBuddyWnd.PopupOpen)
			{
				if (e.KeyCode.Equals(Keys.Tab))
				{
					MakeHelpRequest();
				}
			}
			else if (e.KeyCode.Equals(Keys.Tab))
			{
				m_CodeBuddyWnd.PopupOpen = false;
			}
		}

		private void m_txtTable_TextChanged(object sender, EventArgs e)
		{
			m_UpdateTableTimer.Stop();
			if (m_CodeBuddyWnd.PopupOpen)
			{
				bool flag = false;
				string text = m_txtTable.Text;
				int length = text.Length;
				int length2 = m_sPrevInputText.Length;
				if (length > length2 && length2 > 0)
				{
					string source = text.Replace(m_sPrevInputText, "");
					char[] codeBuddyTriggers = m_CodeBuddyTriggers;
					foreach (char value in codeBuddyTriggers)
					{
						if (source.Contains(value))
						{
							flag = true;
							break;
						}
					}
				}
				else if (length2 > length && length > 0)
				{
					string source2 = m_sPrevInputText.Replace(text, "");
					char[] codeBuddyTriggers2 = m_CodeBuddyTriggers;
					foreach (char value2 in codeBuddyTriggers2)
					{
						if (source2.Contains(value2))
						{
							flag = true;
							break;
						}
					}
				}
				else if (length2 == length)
				{
					flag = true;
				}
				if (flag)
				{
					MakeHelpRequest();
				}
			}
			m_UpdateTableTimer.Start();
		}

		private void m_UpdateTableTimer_Tick(object sender, EventArgs e)
		{
			m_UpdateTableTimer.Stop();
			RefreshTableDisp();
		}

		private void MakeHelpRequest()
		{
			CustomUI customUI = (CustomUI)base.Parent;
			if (customUI != null && customUI.LuaState != null)
			{
				Connection.Instance.Request("HELPT:" + customUI.LuaState.ID + ":Table:" + m_txtTable.Text, m_iHelpNetworkListener);
			}
		}

		private void OnHelpRecieved(List<string> helpStrings)
		{
			UpdateHelpEntries(helpStrings);
			m_sPrevInputText = m_txtTable.Text;
			m_CodeBuddyWnd.PopupOpen = true;
		}

		private void UpdateHelpEntries(List<string> helpStrings)
		{
			List<AutoCompletePopup.Entry> list = new List<AutoCompletePopup.Entry>();
			for (int i = 0; i + 1 < helpStrings.Count; i += 2)
			{
				AutoCompletePopup.Entry item = new AutoCompletePopup.Entry(helpStrings[i], helpStrings[i + 1]);
				list.Add(item);
			}
			m_CodeBuddyWnd.Entries = list;
		}

		private void OnCodeBuddySelection(object sender, AutoCompletePopup.AutoCompleteEventArgs e)
		{
			string text = FindLastSymbol(m_txtTable.Text);
			if (string.IsNullOrEmpty(text))
			{
				m_txtTable.Text += e.Selected;
			}
			else
			{
				string text2 = m_txtTable.Text;
				text2 = text2.Substring(0, text2.Length - text.Length);
				text2 += e.Selected;
				m_txtTable.Text = text2;
			}
			m_txtTable.SelectionStart = m_txtTable.Text.Length;
			m_txtTable.SelectionLength = 0;
		}

		private bool IsSymbolChar(char c)
		{
			if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9'))
			{
				return c == '_';
			}
			return true;
		}

		private string FindLastSymbol(string sLuaCode)
		{
			string text = null;
			for (int num = sLuaCode.Length - 1; num >= 0; num--)
			{
				if (!IsSymbolChar(sLuaCode[num]))
				{
					int num2 = num + 1;
					text = ((num2 >= sLuaCode.Length) ? string.Empty : sLuaCode.Substring(num2));
					break;
				}
			}
			if (text == null)
			{
				text = sLuaCode;
			}
			return text;
		}

		private void m_TableDisp_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			string text = string.Empty;
			for (TreeNode node = e.Node; node != null; node = node.Parent)
			{
				int result;
				text = ((!int.TryParse(node.Text, out result)) ? ("." + node.Text + text) : ("[" + node.Text + "]" + text));
			}
			text = ((!(Table != string.Empty)) ? text.TrimStart('.') : (Table + text));
			MakeTableRequest(text);
		}

		private void m_TableDisp_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			e.Node.Nodes.Clear();
			e.Node.Nodes.Add("Populating List...");
		}
	}
}
