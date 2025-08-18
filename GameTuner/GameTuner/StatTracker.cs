using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class StatTracker : Panel, ICustomControl
	{
		private ComboBox m_cmbStatSet = new ComboBox();

		private Button m_btnToggleTracking = new Button();

		private RichTextBox m_txtOutput = new RichTextBox();

		private int m_iNetworkListener;

		private ContextMenuStrip m_mnuContext = new ContextMenuStrip();

		private ToolStripMenuItem m_btnCopyText = new ToolStripMenuItem("Copy Text");

		private ToolStripMenuItem m_btnMove = new ToolStripMenuItem("Move");

		private ToolStripMenuItem m_btnResize = new ToolStripMenuItem("Resize");

		private ToolStripMenuItem m_btnDelete = new ToolStripMenuItem("Delete");

		public StatTracker(MouseEventHandler onMouseClick, MouseEventHandler onMouseMove)
		{
			base.Width = 530;
			base.Height = 400;
			base.BorderStyle = BorderStyle.FixedSingle;
			base.MouseMove += onMouseMove;
			base.MouseClick += onMouseClick;
			m_btnToggleTracking.Text = "Start";
			m_btnToggleTracking.Width = 50;
			m_btnToggleTracking.Click += m_btnToggleTracking_Click;
			m_btnToggleTracking.BackColor = SystemColors.Control;
			m_btnToggleTracking.ForeColor = SystemColors.ControlText;
			m_btnToggleTracking.Location = new Point(base.Width - m_btnToggleTracking.Width, 0);
			m_btnToggleTracking.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			base.Controls.Add(m_btnToggleTracking);
			m_cmbStatSet.MouseClick += onMouseClick;
			m_cmbStatSet.MouseMove += onMouseMove;
			m_cmbStatSet.SelectedIndexChanged += m_cmbStatSet_SelectedIndexChanged;
			m_cmbStatSet.Location = new Point(0, 0);
			m_cmbStatSet.Width = base.Width - m_btnToggleTracking.Width;
			m_cmbStatSet.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			base.Controls.Add(m_cmbStatSet);
			m_txtOutput.ReadOnly = true;
			m_txtOutput.MouseClick += onMouseClick;
			m_txtOutput.MouseMove += onMouseMove;
			m_txtOutput.Top = m_cmbStatSet.Height + 2;
			m_txtOutput.Left = 0;
			m_txtOutput.Height = base.Height - m_txtOutput.Top;
			m_txtOutput.Width = base.Width;
			m_txtOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			base.Controls.Add(m_txtOutput);
			m_btnCopyText.Click += m_btnCopyText_Click;
			m_btnMove.Click += m_btnMove_Click;
			m_btnResize.Click += m_btnResize_Click;
			m_btnDelete.Click += m_btnDelete_Click;
			m_mnuContext.Items.Add(m_btnCopyText);
			m_mnuContext.Items.Add(new ToolStripSeparator());
			m_mnuContext.Items.Add(m_btnMove);
			m_mnuContext.Items.Add(m_btnResize);
			m_mnuContext.Items.Add(new ToolStripSeparator());
			m_mnuContext.Items.Add(m_btnDelete);
			ContextMenuStrip = m_mnuContext;
			m_txtOutput.ContextMenuStrip = m_mnuContext;
			m_iNetworkListener = Connection.Instance.AddRequestListener(CompletedAction);
			MakeStatTrackerListRequest();
		}

		public void Release()
		{
			Connection.Instance.RemoveRequestListener(m_iNetworkListener);
		}

		private void MakeStatTrackerListRequest()
		{
			Connection.Instance.Request("STRACKERS:", m_iNetworkListener);
		}

		private void StartTracking(uint uiTrackerID)
		{
			Connection.Instance.Request("START_TRACKER:" + uiTrackerID, m_iNetworkListener);
		}

		private void StopTracking()
		{
			Connection.Instance.Request("STOP_TRACKER:", m_iNetworkListener);
			m_txtOutput.Text = string.Empty;
		}

		public void CompletedAction(List<string> luaMessages)
		{
			if (luaMessages.Count <= 0)
			{
				return;
			}
			if (luaMessages[0] == "TrackerList")
			{
				m_cmbStatSet.Items.Clear();
				luaMessages.RemoveAt(0);
				{
					foreach (string luaMessage in luaMessages)
					{
						m_cmbStatSet.Items.Add(luaMessage);
					}
					return;
				}
			}
			m_txtOutput.Text = luaMessages[0];
		}

		public void LuaStateChanged(LuaState state, LuaState lastState)
		{
		}

		public void StartDrag()
		{
			base.BorderStyle = BorderStyle.Fixed3D;
			m_txtOutput.Visible = false;
			m_btnToggleTracking.Visible = false;
			m_cmbStatSet.Visible = false;
		}

		public void EndDrag()
		{
			base.BorderStyle = BorderStyle.FixedSingle;
			m_txtOutput.Visible = true;
			m_btnToggleTracking.Visible = true;
			m_cmbStatSet.Visible = true;
		}

		public void TabEntered()
		{
		}

		public void TabLeft()
		{
		}

		private void m_cmbStatSet_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_btnToggleTracking.Text == "Stop")
			{
				m_btnToggleTracking_Click(null, null);
			}
		}

		private void m_btnToggleTracking_Click(object sender, EventArgs e)
		{
			if (m_btnToggleTracking.Text == "Start")
			{
				m_btnToggleTracking.Text = "Stop";
				StartTracking((uint)m_cmbStatSet.SelectedIndex);
			}
			else
			{
				m_btnToggleTracking.Text = "Start";
				StopTracking();
			}
		}

		private void m_btnCopyText_Click(object sender, EventArgs e)
		{
			m_txtOutput.Copy();
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
