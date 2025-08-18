using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using GameTuner.Framework;

namespace GameTuner
{
	public class SelectionList : Panel, ICustomControl
	{
		private string m_sPopulateList = string.Empty;

		private string m_sOnSelection = string.Empty;

		private string m_Name;

		private List<string> m_aMessages = new List<string>();

		private ListView m_ListBox = new ListView();

		private Timer m_tmrUpdateList = new Timer();

		private int m_iNetworkListener;

		private ContextMenuStrip m_mnuContext = new ContextMenuStrip();

		private ToolStripMenuItem m_btnEdit = new ToolStripMenuItem("Edit");

		private ToolStripMenuItem m_btnCopy = new ToolStripMenuItem("Copy");

		private ToolStripMenuItem m_btnMove = new ToolStripMenuItem("Move");

		private ToolStripMenuItem m_btnResize = new ToolStripMenuItem("Resize");

		private ToolStripMenuItem m_btnDelete = new ToolStripMenuItem("Delete");

		private bool m_bTabSelected;

		public string ControlName
		{
			get
			{
				return m_Name;
			}
			set
			{
				SetListViewName(value);
			}
		}

		public string PopulateList
		{
			get
			{
				return m_sPopulateList;
			}
			set
			{
				m_sPopulateList = value;
			}
		}

		public string OnSelection
		{
			get
			{
				return m_sOnSelection;
			}
			set
			{
				m_sOnSelection = value;
			}
		}

		public bool Sorted
		{
			get
			{
				return m_ListBox.Sorting != SortOrder.None;
			}
			set
			{
				if (value)
				{
					m_ListBox.Sorting = SortOrder.Ascending;
				}
				else
				{
					m_ListBox.Sorting = SortOrder.None;
				}
			}
		}

		private int ExtractColumnData(string szText, ref List<string> szColumns)
		{
			szColumns.Clear();
			if (szText != null && szText.Length > 0)
			{
				int num = 0;
				while (num < szText.Length)
				{
					int num2 = szText.IndexOf(';', num);
					if (num2 == -1)
					{
						num2 = szText.Length;
					}
					string text = szText.Substring(num, num2 - num);
					szColumns.Add(text.Trim());
					num = num2 + 1;
				}
			}
			return szColumns.Count;
		}

		private void SetListViewName(string szName)
		{
			if (m_Name != null && m_Name.Equals(szName))
			{
				return;
			}
			m_Name = szName;
			m_ListBox.Clear();
			int num = 0;
			int num2 = 0;
			while (num < m_Name.Length)
			{
				string text = null;
				int num3 = 100;
				int num4 = m_Name.IndexOf(':', num);
				int num5 = m_Name.IndexOf(';', num);
				if (num5 == -1)
				{
					num5 = m_Name.Length;
				}
				if (num4 >= 0 && num4 < num5)
				{
					text = m_Name.Substring(num, num4 - num);
					if (num4 + 1 < num5)
					{
						string text2 = m_Name.Substring(num4 + 1, num5 - (num4 + 1));
						text2 = text2.Trim();
						try
						{
							num3 = Convert.ToInt32(text2);
						}
						catch
						{
							num3 = 100;
						}
					}
				}
				else
				{
					text = m_Name.Substring(num, num5 - num);
				}
				text = text.Trim();
				num = num5 + 1;
				if (text != null && text.Length > 0)
				{
					ColumnHeader columnHeader = new ColumnHeader();
					columnHeader.Text = text;
					if (num >= m_Name.Length)
					{
						columnHeader.Width = Math.Max(40, m_ListBox.Width - 4 - num2);
					}
					else
					{
						columnHeader.Width = Math.Max(10, num3);
					}
					num2 += num3;
					m_ListBox.Columns.Add(columnHeader);
				}
			}
		}

		public SelectionList(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
		{
			base.Width = 300;
			base.Height = 400;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.MouseMove += onMouseMove;
			base.MouseClick += onMouseClick;
			m_ListBox.View = View.Details;
			m_ListBox.MouseClick += onMouseClick;
			m_ListBox.MouseMove += onMouseMove;
			m_ListBox.SelectedIndexChanged += m_ListBox_SelectedIndexChanged;
			m_ListBox.Top = 0;
			m_ListBox.Left = 0;
			m_ListBox.Height = base.Height - m_ListBox.Top;
			m_ListBox.Width = base.Width;
			m_ListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			m_ListBox.FullRowSelect = true;
			m_ListBox.MultiSelect = false;
			m_ListBox.HideSelection = false;
			ColumnHeader value = new ColumnHeader
			{
				Text = m_Name,
				Width = base.Width
			};
			m_ListBox.Columns.Add(value);
			base.Controls.Add(m_ListBox);
			m_btnEdit.Click += m_btnEdit_Click;
			m_btnCopy.Click += m_btnCopy_Click;
			m_btnMove.Click += m_btnMove_Click;
			m_btnResize.Click += m_btnResize_Click;
			m_btnDelete.Click += m_btnDelete_Click;
			m_mnuContext.Items.Add(m_btnEdit);
			m_mnuContext.Items.Add(m_btnCopy);
			m_mnuContext.Items.Add(m_btnMove);
			m_mnuContext.Items.Add(m_btnResize);
			m_mnuContext.Items.Add(new ToolStripSeparator());
			m_mnuContext.Items.Add(m_btnDelete);
			ContextMenuStrip = m_mnuContext;
			m_ListBox.ContextMenuStrip = m_mnuContext;
			m_tmrUpdateList.Interval = 1000;
			m_tmrUpdateList.Tick += m_tmrUpdateList_Tick;
			m_iNetworkListener = Connection.Instance.AddRequestListener(CompletedAction);
		}

		public void Release()
		{
			Connection.Instance.RemoveRequestListener(m_iNetworkListener);
		}

		public void TabEntered()
		{
			m_bTabSelected = true;
			UpdateList();
		}

		public void TabLeft()
		{
			m_tmrUpdateList.Enabled = false;
			m_bTabSelected = false;
		}

		private void m_tmrUpdateList_Tick(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void UpdateList()
		{
			if (base.Parent != null && ((CustomUI)base.Parent).LuaState != null)
			{
				LuaState luaState = ((CustomUI)base.Parent).LuaState;
				Connection.Instance.Request("LIST:" + luaState.ID + ":" + PopulateList, m_iNetworkListener);
			}
		}

		public void CompletedAction(List<string> luaMessages)
		{
			if (luaMessages.Count > 0)
			{
				if (luaMessages[0] == "OnSelection Error")
				{
					if (luaMessages.Count > 1)
					{
						MessageBox.Show("Error in OnSelection:\n" + luaMessages[1]);
					}
					return;
				}
				m_tmrUpdateList.Enabled = false;
				if (luaMessages[0] == "Populate List Error")
				{
					if (luaMessages.Count > 1)
					{
						MessageBox.Show("Failed to populate list:\n" + luaMessages[1]);
					}
					return;
				}
				luaMessages.RemoveAt(0);
				bool flag = false;
				if (luaMessages.Count != m_ListBox.Items.Count)
				{
					flag = true;
				}
				else
				{
					if (Sorted)
					{
						luaMessages.Sort();
					}
					for (int i = 0; i < luaMessages.Count; i++)
					{
						if (!luaMessages[i].Equals(m_aMessages[i]))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					m_ListBox.BeginUpdate();
					m_aMessages.Clear();
					string text = null;
					if (m_ListBox.SelectedItems.Count != 0)
					{
						text = m_ListBox.SelectedItems[0].Text;
					}
					m_ListBox.Items.Clear();
					List<string> szColumns = new List<string>();
					foreach (string luaMessage in luaMessages)
					{
						m_aMessages.Add(luaMessage);
						ExtractColumnData(luaMessage, ref szColumns);
						ListViewItem listViewItem = new ListViewItem();
						if (szColumns.Count > 0)
						{
							listViewItem.Text = szColumns[0];
							for (int j = 1; j < szColumns.Count; j++)
							{
								listViewItem.SubItems.Add(szColumns[j]);
							}
						}
						m_ListBox.Items.Add(listViewItem);
					}
					if (text != null)
					{
						ListViewItem[] array = m_ListBox.Items.Find(text, false);
						if (array.Length != 0)
						{
							m_ListBox.SelectedIndices.Add(array[0].Index);
						}
						else if (m_ListBox.Items.Count > 0)
						{
							m_ListBox.SelectedIndices.Add(0);
						}
					}
					else if (m_ListBox.Items.Count > 0)
					{
						m_ListBox.SelectedIndices.Add(0);
					}
					m_ListBox.EndUpdate();
				}
			}
			m_tmrUpdateList.Enabled = true;
		}

		public void LuaStateChanged(LuaState state, LuaState lastState)
		{
			if (m_bTabSelected)
			{
				UpdateList();
			}
		}

		private void m_ListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_ListBox.SelectedItems.Count != 0 && base.Parent != null && ((CustomUI)base.Parent).LuaState != null)
			{
				LuaState luaState = ((CustomUI)base.Parent).LuaState;
				Connection.Instance.Request("LISTSEL:" + luaState.ID + ":" + m_ListBox.SelectedItems[0].Text + ":" + OnSelection, m_iNetworkListener);
			}
		}

		public void StartDrag()
		{
			base.BorderStyle = BorderStyle.Fixed3D;
			m_ListBox.Visible = false;
		}

		public void EndDrag()
		{
			base.BorderStyle = BorderStyle.FixedSingle;
			m_ListBox.Visible = true;
		}

		private void m_btnEdit_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.EditSelectionListControl(this);
		}

		private void m_btnCopy_Click(object sender, EventArgs e)
		{
			StringWriter stringWriter = new StringWriter();
			try
			{
				CustomUI.PanelData.SelectionListData data = new CustomUI.PanelData.SelectionListData(this);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GenericObjectSerializer));
				xmlSerializer.Serialize(stringWriter, new GenericObjectSerializer(data));
				Clipboard.SetText(stringWriter.ToString());
			}
			catch (Exception ex)
			{
				ErrorHandling.Error(ex, "Failed to copy control:\n" + ex.Message, ErrorLevel.ShowMessage);
			}
			finally
			{
				stringWriter.Close();
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
	}
}
