using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class PickAssetPathForm : Form
	{
		private bool allowManualEdit;

		private IContainer components;

		private CaptionControl captionControl1;

		private Label label1;

		private Button buttonOk;

		private Button buttonClear;

		private Label label2;

		private TextBox textBox1;

		private Button button1;

		private FolderBrowserDialog folderDialog;

		private CheckBox btnEditManually;

		private Panel panel1;

		public string BasePath { get; private set; }

		[DefaultValue(false)]
		[Category("Behavior")]
		public bool AllowManualEdit
		{
			get
			{
				return allowManualEdit;
			}
			set
			{
				allowManualEdit = value;
				btnEditManually.Visible = allowManualEdit;
			}
		}

		public PickAssetPathForm(string basePath)
		{
			BasePath = basePath;
			InitializeComponent();
			textBox1.Text = BasePath ?? "";
			buttonOk.Enabled = !string.IsNullOrEmpty(BasePath);
			AllowManualEdit = false;
		}

		private void PickAssetPathForm_Load(object sender, EventArgs e)
		{
			captionControl1.Image = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();
			captionControl1.Caption = Application.ProductName;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			folderDialog.SelectedPath = BasePath ?? "";
			if (folderDialog.ShowDialog(this) == DialogResult.OK)
			{
				BasePath = folderDialog.SelectedPath;
				textBox1.Text = BasePath;
				buttonOk.Enabled = !string.IsNullOrEmpty(BasePath);
			}
		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			BasePath = null;
			textBox1.Text = "";
			buttonOk.Enabled = false;
		}

		private void btnEditManually_CheckedChanged(object sender, EventArgs e)
		{
			textBox1.ReadOnly = !btnEditManually.Checked;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (btnEditManually.Checked)
			{
				BasePath = textBox1.Text;
				buttonOk.Enabled = !string.IsNullOrEmpty(BasePath);
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
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonClear = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.btnEditManually = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.captionControl1 = new GameTuner.Framework.CaptionControl();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(300, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "This application requires the location where assets are located";
			this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Enabled = false;
			this.buttonOk.Location = new System.Drawing.Point(270, 9);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this.buttonClear.Location = new System.Drawing.Point(15, 9);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(75, 23);
			this.buttonClear.TabIndex = 4;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(buttonClear_Click);
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 81);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Base Path";
			this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.textBox1.Location = new System.Drawing.Point(74, 78);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(240, 20);
			this.textBox1.TabIndex = 6;
			this.textBox1.TextChanged += new System.EventHandler(textBox1_TextChanged);
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.button1.Location = new System.Drawing.Point(320, 75);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(25, 23);
			this.button1.TabIndex = 7;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(button1_Click);
			this.folderDialog.Description = "Select Base Path";
			this.btnEditManually.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.btnEditManually.AutoSize = true;
			this.btnEditManually.Location = new System.Drawing.Point(232, 104);
			this.btnEditManually.Name = "btnEditManually";
			this.btnEditManually.Size = new System.Drawing.Size(82, 17);
			this.btnEditManually.TabIndex = 8;
			this.btnEditManually.Text = "Manual Edit";
			this.btnEditManually.UseVisualStyleBackColor = true;
			this.btnEditManually.CheckedChanged += new System.EventHandler(btnEditManually_CheckedChanged);
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel1.Controls.Add(this.buttonOk);
			this.panel1.Controls.Add(this.buttonClear);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 129);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(357, 44);
			this.panel1.TabIndex = 9;
			this.captionControl1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.captionControl1.Caption = "Application Name";
			this.captionControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.captionControl1.Image = GameTuner.Framework.Properties.Resources.dir_closed;
			this.captionControl1.Location = new System.Drawing.Point(0, 0);
			this.captionControl1.Name = "captionControl1";
			this.captionControl1.Size = new System.Drawing.Size(357, 37);
			this.captionControl1.TabIndex = 0;
			this.captionControl1.Transparent = System.Drawing.Color.Magenta;
			base.AcceptButton = this.buttonOk;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(357, 173);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.btnEditManually);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.captionControl1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PickAssetPathForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Pick Asset Path";
			base.TopMost = true;
			base.Load += new System.EventHandler(PickAssetPathForm_Load);
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
