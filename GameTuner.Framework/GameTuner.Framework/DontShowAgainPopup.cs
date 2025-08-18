using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public class DontShowAgainPopup : Form
	{
		private IContainer components;

		private Button buttonOK;

		private CheckBox checkBox;

		private RichTextBox messageBox;

		public string Message
		{
			get
			{
				return messageBox.Text;
			}
			set
			{
				messageBox.Text = value;
			}
		}

		public string MessageHtml
		{
			set
			{
				messageBox.Rtf = StringHelper.HTMLToRTF(value);
			}
		}

		public bool DontShowAgain
		{
			get
			{
				return checkBox.Checked;
			}
			set
			{
				checkBox.Checked = value;
			}
		}

		public DontShowAgainPopup()
		{
			InitializeComponent();
			DontShowAgain = false;
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.checkBox = new System.Windows.Forms.CheckBox();
			this.messageBox = new System.Windows.Forms.RichTextBox();
			base.SuspendLayout();
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(179, 75);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.checkBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.checkBox.AutoSize = true;
			this.checkBox.Location = new System.Drawing.Point(12, 81);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new System.Drawing.Size(127, 17);
			this.checkBox.TabIndex = 1;
			this.checkBox.Text = "Don't show this again";
			this.checkBox.UseVisualStyleBackColor = true;
			this.messageBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.messageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.messageBox.Location = new System.Drawing.Point(12, 12);
			this.messageBox.Name = "messageBox";
			this.messageBox.ReadOnly = true;
			this.messageBox.Size = new System.Drawing.Size(242, 57);
			this.messageBox.TabIndex = 2;
			this.messageBox.Text = "Don't show again message goes here";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(266, 110);
			base.Controls.Add(this.messageBox);
			base.Controls.Add(this.checkBox);
			base.Controls.Add(this.buttonOK);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(247, 120);
			base.Name = "DontShowAgainPopup";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "DontShowAgainPopup";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
