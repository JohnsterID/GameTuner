using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class PanelBuilder : Form
	{
		private string m_sEnterAction = string.Empty;

		private string m_sExitAction = string.Empty;

		private IContainer components;

		private Label lblName;

		private TextBox txtName;

		private ListView lstLuaStates;

		private Button btnOK;

		private Button btnCancel;

		private Button m_btnEnterAction;

		private Button m_btnExitAction;

		public string PanelName
		{
			get
			{
				return txtName.Text;
			}
			set
			{
				txtName.Text = value;
			}
		}

		public string EnterAction
		{
			get
			{
				return m_sEnterAction;
			}
			set
			{
				m_sEnterAction = value;
			}
		}

		public string ExitAction
		{
			get
			{
				return m_sExitAction;
			}
			set
			{
				m_sExitAction = value;
			}
		}

		public List<string> CompatibleStates
		{
			get
			{
				List<string> list = new List<string>();
				foreach (ListViewItem item in lstLuaStates.Items)
				{
					if (item.Checked)
					{
						list.Add(item.Text);
					}
				}
				return list;
			}
			set
			{
				foreach (string item in value)
				{
					ListViewItem[] array = lstLuaStates.Items.Find(item, true);
					if (array.Length > 0)
					{
						array[0].Checked = true;
						continue;
					}
					ListViewItem listViewItem = lstLuaStates.Items.Add(item, item, -1);
					listViewItem.Checked = true;
				}
			}
		}

		public IEnumerable<string> AvailableLuaStates
		{
			set
			{
				lstLuaStates.Items.Clear();
				foreach (string item in value)
				{
					lstLuaStates.Items.Add(item, item, -1);
				}
			}
		}

		public PanelBuilder()
		{
			InitializeComponent();
		}

		private LuaState FindValidState()
		{
			List<string> compatibleStates = CompatibleStates;
			LuaState result = null;
			foreach (LuaState luaState in LuaStateManager.Instance.LuaStates)
			{
				if (compatibleStates.Contains(luaState.Name))
				{
					result = luaState;
					break;
				}
			}
			return result;
		}

		private void m_btnEnterAction_Click(object sender, EventArgs e)
		{
			LuaState luaState = FindValidState();
			if (luaState == null)
			{
				MessageBox.Show("Can't edit on enter action.  Valid lua state not found");
				return;
			}
			ActionBuilder actionBuilder = new ActionBuilder();
			actionBuilder.LuaStateID = luaState.ID.ToString();
			actionBuilder.FixedName = true;
			actionBuilder.ActionName = "On Enter " + PanelName;
			actionBuilder.ActionCode = EnterAction;
			if (actionBuilder.ShowDialog() == DialogResult.OK)
			{
				EnterAction = actionBuilder.ActionCode;
			}
		}

		private void m_btnExitAction_Click(object sender, EventArgs e)
		{
			LuaState luaState = FindValidState();
			if (luaState == null)
			{
				MessageBox.Show("Can't edit on enter action.  Valid lua state not found");
				return;
			}
			ActionBuilder actionBuilder = new ActionBuilder();
			actionBuilder.LuaStateID = luaState.ID.ToString();
			actionBuilder.FixedName = true;
			actionBuilder.ActionName = "On Exit " + PanelName;
			actionBuilder.ActionCode = ExitAction;
			if (actionBuilder.ShowDialog() == DialogResult.OK)
			{
				ExitAction = actionBuilder.ActionCode;
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
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.lstLuaStates = new System.Windows.Forms.ListView();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.m_btnEnterAction = new System.Windows.Forms.Button();
			this.m_btnExitAction = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(8, 10);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(38, 13);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "Name:";
			this.txtName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtName.BackColor = System.Drawing.Color.Black;
			this.txtName.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.txtName.ForeColor = System.Drawing.Color.White;
			this.txtName.Location = new System.Drawing.Point(52, 5);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(375, 22);
			this.txtName.TabIndex = 1;
			this.txtName.WordWrap = false;
			this.lstLuaStates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.lstLuaStates.BackColor = System.Drawing.Color.Black;
			this.lstLuaStates.CheckBoxes = true;
			this.lstLuaStates.ForeColor = System.Drawing.Color.White;
			this.lstLuaStates.Location = new System.Drawing.Point(52, 33);
			this.lstLuaStates.Name = "lstLuaStates";
			this.lstLuaStates.Size = new System.Drawing.Size(375, 68);
			this.lstLuaStates.TabIndex = 2;
			this.lstLuaStates.UseCompatibleStateImageBehavior = false;
			this.lstLuaStates.View = System.Windows.Forms.View.List;
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(293, 107);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(64, 27);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(363, 107);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 27);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.m_btnEnterAction.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.m_btnEnterAction.BackColor = System.Drawing.SystemColors.Control;
			this.m_btnEnterAction.ForeColor = System.Drawing.SystemColors.ControlText;
			this.m_btnEnterAction.Location = new System.Drawing.Point(52, 107);
			this.m_btnEnterAction.Name = "m_btnEnterAction";
			this.m_btnEnterAction.Size = new System.Drawing.Size(68, 27);
			this.m_btnEnterAction.TabIndex = 5;
			this.m_btnEnterAction.Text = "On Enter";
			this.m_btnEnterAction.UseVisualStyleBackColor = false;
			this.m_btnEnterAction.Click += new System.EventHandler(m_btnEnterAction_Click);
			this.m_btnExitAction.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.m_btnExitAction.BackColor = System.Drawing.SystemColors.Control;
			this.m_btnExitAction.ForeColor = System.Drawing.SystemColors.ControlText;
			this.m_btnExitAction.Location = new System.Drawing.Point(126, 107);
			this.m_btnExitAction.Name = "m_btnExitAction";
			this.m_btnExitAction.Size = new System.Drawing.Size(68, 27);
			this.m_btnExitAction.TabIndex = 6;
			this.m_btnExitAction.Text = "On Exit";
			this.m_btnExitAction.UseVisualStyleBackColor = false;
			this.m_btnExitAction.Click += new System.EventHandler(m_btnExitAction_Click);
			base.AcceptButton = this.btnOK;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(439, 148);
			base.Controls.Add(this.m_btnExitAction);
			base.Controls.Add(this.m_btnEnterAction);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.lstLuaStates);
			base.Controls.Add(this.txtName);
			base.Controls.Add(this.lblName);
			this.ForeColor = System.Drawing.Color.White;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PanelBuilder";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Panel Builder";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
