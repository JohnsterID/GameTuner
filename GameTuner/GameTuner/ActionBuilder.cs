using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner
{
	public class ActionBuilder : Form
	{
		private string m_sLuaStateID = "0";

		private bool m_bBadCode;

		private IContainer components;

		private Label lblName;

		private TextBox txtName;

		private Label lblAction;

		private Button btnOK;

		private Button btnCancel;

		private Label lblOutput;

		private RichTextBox txtOutput;

		private CodeEditBox txtAction;

		private Button btnTestAction;

		public string ActionName
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

		public string ActionCode
		{
			get
			{
				return txtAction.Text;
			}
			set
			{
				txtAction.Text = value;
			}
		}

		public string LuaStateID
		{
			set
			{
				m_sLuaStateID = value;
			}
		}

		public bool FixedName
		{
			get
			{
				return txtName.ReadOnly;
			}
			set
			{
				txtName.ReadOnly = value;
			}
		}

		public bool BadCode
		{
			get
			{
				return m_bBadCode;
			}
		}

		public ActionBuilder()
		{
			InitializeComponent();
		}

		private void btnTestAction_Click(object sender, EventArgs e)
		{
			Connection.Instance.Request("CMD:" + m_sLuaStateID + ":" + txtAction.Text, ActionTestComplete);
		}

		private void ActionTestComplete(List<string> results)
		{
			txtOutput.Text = results[0];
			if (txtOutput.Text.StartsWith("ERR:"))
			{
				m_bBadCode = true;
				txtOutput.ForeColor = Color.Red;
			}
			else
			{
				m_bBadCode = false;
				txtOutput.ForeColor = Color.White;
			}
			if (txtOutput.Text == string.Empty)
			{
				txtOutput.Text = "Command ran without errors.";
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
			this.lblAction = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblOutput = new System.Windows.Forms.Label();
			this.txtOutput = new System.Windows.Forms.RichTextBox();
			this.btnTestAction = new System.Windows.Forms.Button();
			this.txtAction = new CodeEditBox();
			base.SuspendLayout();
			this.lblName.AutoSize = true;
			this.lblName.BackColor = System.Drawing.Color.Transparent;
			this.lblName.ForeColor = System.Drawing.Color.White;
			this.lblName.Location = new System.Drawing.Point(16, 9);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(38, 13);
			this.lblName.TabIndex = 1;
			this.lblName.Text = "Name:";
			this.txtName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtName.BackColor = System.Drawing.Color.Black;
			this.txtName.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.txtName.ForeColor = System.Drawing.Color.White;
			this.txtName.Location = new System.Drawing.Point(60, 7);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(549, 22);
			this.txtName.TabIndex = 0;
			this.txtName.WordWrap = false;
			this.lblAction.AutoSize = true;
			this.lblAction.BackColor = System.Drawing.Color.Transparent;
			this.lblAction.ForeColor = System.Drawing.Color.White;
			this.lblAction.Location = new System.Drawing.Point(16, 36);
			this.lblAction.Name = "lblAction";
			this.lblAction.Size = new System.Drawing.Size(40, 13);
			this.lblAction.TabIndex = 3;
			this.lblAction.Text = "Action:";
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnOK.Location = new System.Drawing.Point(484, 326);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(60, 26);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnCancel.Location = new System.Drawing.Point(550, 326);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(60, 26);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.lblOutput.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.lblOutput.AutoSize = true;
			this.lblOutput.BackColor = System.Drawing.Color.Transparent;
			this.lblOutput.ForeColor = System.Drawing.Color.White;
			this.lblOutput.Location = new System.Drawing.Point(18, 255);
			this.lblOutput.Name = "lblOutput";
			this.lblOutput.Size = new System.Drawing.Size(42, 13);
			this.lblOutput.TabIndex = 4;
			this.lblOutput.Text = "Output:";
			this.txtOutput.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtOutput.BackColor = System.Drawing.Color.Black;
			this.txtOutput.ForeColor = System.Drawing.Color.White;
			this.txtOutput.Location = new System.Drawing.Point(60, 255);
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.Size = new System.Drawing.Size(549, 65);
			this.txtOutput.TabIndex = 2;
			this.txtOutput.Text = "";
			this.txtOutput.WordWrap = false;
			this.btnTestAction.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.btnTestAction.BackColor = System.Drawing.SystemColors.Control;
			this.btnTestAction.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnTestAction.Location = new System.Drawing.Point(60, 326);
			this.btnTestAction.Name = "btnTestAction";
			this.btnTestAction.Size = new System.Drawing.Size(67, 26);
			this.btnTestAction.TabIndex = 3;
			this.btnTestAction.Text = "Test";
			this.btnTestAction.UseVisualStyleBackColor = false;
			this.btnTestAction.Click += new System.EventHandler(btnTestAction_Click);
			this.txtAction.AcceptsTab = true;
			this.txtAction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.txtAction.BackColor = System.Drawing.Color.Black;
			this.txtAction.ForeColor = System.Drawing.Color.White;
			this.txtAction.Location = new System.Drawing.Point(60, 35);
			this.txtAction.Name = "txtAction";
			this.txtAction.Size = new System.Drawing.Size(549, 214);
			this.txtAction.TabIndex = 1;
			this.txtAction.Text = "";
			this.txtAction.WordWrap = false;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new System.Drawing.Size(624, 364);
			base.Controls.Add(this.btnTestAction);
			base.Controls.Add(this.txtAction);
			base.Controls.Add(this.txtOutput);
			base.Controls.Add(this.lblOutput);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.lblAction);
			base.Controls.Add(this.txtName);
			base.Controls.Add(this.lblName);
			this.ForeColor = System.Drawing.Color.White;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ActionBuilder";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Action Builder";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
