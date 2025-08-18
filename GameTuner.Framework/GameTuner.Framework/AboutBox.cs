using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;
using Microsoft.Win32;

namespace GameTuner.Framework
{
	public class AboutBox : Form
	{
		private string appName = "";

		private string wikiLink = "";

		private string version = "";

		private IContainer components;

		private CaptionControl captionControl;

		private Button buttonOK;

		private LinkLabel linkLabel;

		private Label label1;

		private Label labelVersion;

		public AboutBox()
		{
			InitializeComponent();
			appName = Application.ProductName;
			wikiLink = GameTuner.Framework.Properties.Resources.ToolsWikiBase + "/" + appName;
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.CurrentUser.OpenSubKey(GameTuner.Framework.Properties.Resources.ToolsRegKey + "\\" + appName);
				if (registryKey != null)
				{
					version = "v" + (string)registryKey.GetValue("Version");
				}
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			captionControl.Caption = "About " + appName;
			labelVersion.Text = version;
			linkLabel.Text = appName + " Wiki";
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(wikiLink);
			Process.Start(startInfo);
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
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.captionControl = new GameTuner.Framework.CaptionControl();
			base.SuspendLayout();
			this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(228, 97);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.linkLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.linkLabel.AutoSize = true;
			this.linkLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 80);
			this.linkLabel.Location = new System.Drawing.Point(12, 102);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(73, 17);
			this.linkLabel.TabIndex = 3;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "{app wiki link}";
			this.linkLabel.UseCompatibleTextRendering = true;
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 77);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(162, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Open Source Game Tuning Tool";
			this.labelVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.labelVersion.AutoSize = true;
			this.labelVersion.Location = new System.Drawing.Point(9, 49);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(70, 13);
			this.labelVersion.TabIndex = 1;
			this.labelVersion.Text = "{app version}";
			this.captionControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.captionControl.Caption = "About {application}";
			this.captionControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.captionControl.Image = null;
			this.captionControl.Location = new System.Drawing.Point(0, 0);
			this.captionControl.Name = "captionControl";
			this.captionControl.Size = new System.Drawing.Size(315, 37);
			this.captionControl.TabIndex = 0;
			this.captionControl.Transparent = System.Drawing.Color.Magenta;
			base.AcceptButton = this.buttonOK;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(315, 132);
			base.Controls.Add(this.labelVersion);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.linkLabel);
			base.Controls.Add(this.buttonOK);
			base.Controls.Add(this.captionControl);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AboutBox";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AboutBox";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
