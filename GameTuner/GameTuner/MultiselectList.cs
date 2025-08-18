using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using GameTuner.Framework;

namespace GameTuner
{
	public class MultiselectList : UserControl, ICustomControl
	{
		private string m_sPopulateList = string.Empty;

		private string m_sOnSelected = string.Empty;

		private string m_sOnDeslected = string.Empty;

		private int m_iNetworkListener;

		private bool m_bTabSelected;

		private bool m_bUpdatingList;

		private List<object> m_PrevSelection = new List<object>();

		private IContainer components;

		private Label m_lblName;

		private ListBox m_ListBox;

		private Timer m_tmrUpdateList;

		private ContextMenuStrip m_mnuContext;

		private ToolStripMenuItem m_btnEdit;

		private ToolStripMenuItem m_btnCopy;

		private ToolStripMenuItem m_btnMove;

		private ToolStripMenuItem m_Resize;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem m_btnDelete;

		public string ControlName
		{
			get
			{
				return m_lblName.Text;
			}
			set
			{
				m_lblName.Text = value;
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

		public string OnSelected
		{
			get
			{
				return m_sOnSelected;
			}
			set
			{
				m_sOnSelected = value;
			}
		}

		public string OnDeslected
		{
			get
			{
				return m_sOnDeslected;
			}
			set
			{
				m_sOnDeslected = value;
			}
		}

		public MultiselectList(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
		{
			InitializeComponent();
			base.MouseMove += onMouseMove;
			base.MouseClick += onMouseClick;
			m_lblName.MouseClick += onMouseClick;
			m_lblName.MouseMove += onMouseMove;
			m_ListBox.MouseClick += onMouseClick;
			m_ListBox.MouseMove += onMouseMove;
			m_ListBox.ContextMenuStrip = m_mnuContext;
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
				Connection.Instance.Request("MLIST:" + luaState.ID + ":" + PopulateList, m_iNetworkListener);
				m_tmrUpdateList.Enabled = false;
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
						MessageBox.Show("Failed to update selection:\n" + luaMessages[1]);
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
				m_bUpdatingList = true;
				bool flag = false;
				if (luaMessages.Count / 2 != m_ListBox.Items.Count)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < luaMessages.Count; i += 2)
					{
						string value = luaMessages[i];
						if (!m_ListBox.Items.Contains(value))
						{
							flag = true;
							break;
						}
					}
				}
				m_ListBox.BeginUpdate();
				if (flag)
				{
					m_ListBox.SelectedItems.Clear();
					m_ListBox.Items.Clear();
					for (int j = 0; j < luaMessages.Count; j += 2)
					{
						string text = luaMessages[j];
						m_ListBox.Items.Add(text);
						if (luaMessages[j + 1] == "t")
						{
							m_ListBox.SelectedItems.Add(text);
						}
					}
				}
				else
				{
					for (int k = 0; k < luaMessages.Count; k += 2)
					{
						string text2 = luaMessages[k];
						if (luaMessages[k + 1] == "t")
						{
							if (!m_ListBox.SelectedItems.Contains(text2))
							{
								m_ListBox.SelectedItems.Add(text2);
							}
						}
						else if (m_ListBox.SelectedItems.Contains(text2))
						{
							m_ListBox.SelectedItems.Remove(text2);
						}
					}
				}
				m_ListBox.EndUpdate();
				m_PrevSelection.Clear();
				foreach (object selectedItem in m_ListBox.SelectedItems)
				{
					m_PrevSelection.Add(selectedItem);
				}
				m_bUpdatingList = false;
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

		private void m_ListBox_SelectedValueChanged(object sender, EventArgs e)
		{
			if (m_bUpdatingList)
			{
				return;
			}
			if (m_ListBox.SelectedItems.Count > m_PrevSelection.Count)
			{
				foreach (object selectedItem in m_ListBox.SelectedItems)
				{
					if (!m_PrevSelection.Contains(selectedItem))
					{
						if (m_ListBox.SelectedItem != null && base.Parent != null && ((CustomUI)base.Parent).LuaState != null)
						{
							LuaState luaState = ((CustomUI)base.Parent).LuaState;
							Connection.Instance.Request("LISTSEL:" + luaState.ID + ":" + selectedItem.ToString() + ":" + OnSelected, m_iNetworkListener);
						}
						UpdateList();
						break;
					}
				}
			}
			else
			{
				foreach (object item in m_PrevSelection)
				{
					if (!m_ListBox.SelectedItems.Contains(item))
					{
						if (m_ListBox.SelectedItem != null && base.Parent != null && ((CustomUI)base.Parent).LuaState != null)
						{
							LuaState luaState2 = ((CustomUI)base.Parent).LuaState;
							Connection.Instance.Request("LISTSEL:" + luaState2.ID + ":" + item.ToString() + ":" + OnDeslected, m_iNetworkListener);
						}
						UpdateList();
						break;
					}
				}
			}
			m_PrevSelection.Clear();
			foreach (object selectedItem2 in m_ListBox.SelectedItems)
			{
				m_PrevSelection.Add(selectedItem2);
			}
		}

		private void m_btnEdit_Click(object sender, EventArgs e)
		{
			CustomUI customUI = (CustomUI)base.Parent;
			customUI.EditMultiselectListControl(this);
		}

		private void m_btnCopy_Click(object sender, EventArgs e)
		{
			StringWriter stringWriter = new StringWriter();
			try
			{
				CustomUI.PanelData.MultiselectListData data = new CustomUI.PanelData.MultiselectListData(this);
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

		private void m_Resize_Click(object sender, EventArgs e)
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

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.m_lblName = new System.Windows.Forms.Label();
			this.m_ListBox = new System.Windows.Forms.ListBox();
			this.m_tmrUpdateList = new System.Windows.Forms.Timer(this.components);
			this.m_mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.m_btnEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_btnCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.m_btnMove = new System.Windows.Forms.ToolStripMenuItem();
			this.m_Resize = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_btnDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.m_mnuContext.SuspendLayout();
			base.SuspendLayout();
			this.m_lblName.BackColor = System.Drawing.Color.LightBlue;
			this.m_lblName.Dock = System.Windows.Forms.DockStyle.Top;
			this.m_lblName.ForeColor = System.Drawing.Color.Black;
			this.m_lblName.Location = new System.Drawing.Point(0, 0);
			this.m_lblName.Name = "m_lblName";
			this.m_lblName.Size = new System.Drawing.Size(254, 13);
			this.m_lblName.TabIndex = 0;
			this.m_ListBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.m_ListBox.FormattingEnabled = true;
			this.m_ListBox.IntegralHeight = false;
			this.m_ListBox.Location = new System.Drawing.Point(0, 13);
			this.m_ListBox.Name = "m_ListBox";
			this.m_ListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.m_ListBox.Size = new System.Drawing.Size(254, 242);
			this.m_ListBox.Sorted = true;
			this.m_ListBox.TabIndex = 1;
			this.m_ListBox.SelectedValueChanged += new System.EventHandler(m_ListBox_SelectedValueChanged);
			this.m_tmrUpdateList.Interval = 1000;
			this.m_tmrUpdateList.Tick += new System.EventHandler(m_tmrUpdateList_Tick);
			this.m_mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.m_btnEdit, this.m_btnCopy, this.m_btnMove, this.m_Resize, this.toolStripSeparator1, this.m_btnDelete });
			this.m_mnuContext.Name = "m_mnuContext";
			this.m_mnuContext.Size = new System.Drawing.Size(153, 142);
			this.m_btnEdit.Name = "m_btnEdit";
			this.m_btnEdit.Size = new System.Drawing.Size(152, 22);
			this.m_btnEdit.Text = "Edit";
			this.m_btnEdit.Click += new System.EventHandler(m_btnEdit_Click);
			this.m_btnCopy.Name = "m_btnCopy";
			this.m_btnCopy.Size = new System.Drawing.Size(152, 22);
			this.m_btnCopy.Text = "Copy";
			this.m_btnCopy.Click += new System.EventHandler(m_btnCopy_Click);
			this.m_btnMove.Name = "m_btnMove";
			this.m_btnMove.Size = new System.Drawing.Size(152, 22);
			this.m_btnMove.Text = "Move";
			this.m_btnMove.Click += new System.EventHandler(m_btnMove_Click);
			this.m_Resize.Name = "m_Resize";
			this.m_Resize.Size = new System.Drawing.Size(152, 22);
			this.m_Resize.Text = "Resize";
			this.m_Resize.Click += new System.EventHandler(m_Resize_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			this.m_btnDelete.Name = "m_btnDelete";
			this.m_btnDelete.Size = new System.Drawing.Size(152, 22);
			this.m_btnDelete.Text = "Delete";
			this.m_btnDelete.Click += new System.EventHandler(m_btnDelete_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			base.Controls.Add(this.m_ListBox);
			base.Controls.Add(this.m_lblName);
			this.ForeColor = System.Drawing.Color.White;
			base.Name = "MultiselectList";
			base.Size = new System.Drawing.Size(254, 254);
			this.m_mnuContext.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
