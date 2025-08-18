using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GameTuner.Framework.Controls
{
	public class FolderSelectionControl : UserControl
	{
		public class FolderChangeEventArgs : EventArgs
		{
			public string Folder { get; private set; }

			public bool Valid { get; private set; }

			public FolderChangeEventArgs(string Folder, bool Valid)
			{
				this.Folder = Folder;
				this.Valid = Valid;
			}
		}

		public delegate void FolderChangeEventHandler(object sender, FolderChangeEventArgs e);

		private string m_szFolder = "";

		public FolderChangeEventHandler FolderChange;

		private IContainer components;

		private FlowLayoutPanel flowLayoutPanel1;

		private TextBox FolderTextBox;

		private Button FolderBrowseButton;

		private FolderBrowserDialog FolderBrowseDialog;

		public string Folder
		{
			get
			{
				return m_szFolder;
			}
			set
			{
				m_szFolder = value;
				bool validFolder = ValidFolder;
				FolderTextBox.BackColor = (validFolder ? ValidFolderColor : InvalidFolderColor);
				if (FolderChange != null)
				{
					FolderChange(this, new FolderChangeEventArgs(m_szFolder, validFolder));
				}
			}
		}

		public Color ValidFolderColor { get; set; }

		public Color InvalidFolderColor { get; set; }

		public bool ValidFolder
		{
			get
			{
				return Directory.Exists(m_szFolder);
			}
		}

		public FolderSelectionControl()
		{
			InitializeComponent();
			ValidFolderColor = Color.LightGreen;
			InvalidFolderColor = Color.Red;
		}

		private void FolderBrowseButton_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = FolderBrowseDialog.ShowDialog(this);
			if (dialogResult == DialogResult.OK)
			{
				Folder = FolderBrowseDialog.SelectedPath;
			}
		}

		private void FolderTextBox_TextChanged(object sender, EventArgs e)
		{
			Folder = (sender as TextBox).Text;
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.FolderTextBox = new System.Windows.Forms.TextBox();
			this.FolderBrowseButton = new System.Windows.Forms.Button();
			this.FolderBrowseDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.flowLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.FolderTextBox);
			this.flowLayoutPanel1.Controls.Add(this.FolderBrowseButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(266, 26);
			this.flowLayoutPanel1.TabIndex = 0;
			this.flowLayoutPanel1.WrapContents = false;
			this.FolderTextBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.FolderTextBox.Location = new System.Drawing.Point(3, 3);
			this.FolderTextBox.Name = "FolderTextBox";
			this.FolderTextBox.Size = new System.Drawing.Size(218, 20);
			this.FolderTextBox.TabIndex = 1;
			this.FolderTextBox.TextChanged += new System.EventHandler(FolderTextBox_TextChanged);
			this.FolderBrowseButton.Location = new System.Drawing.Point(227, 3);
			this.FolderBrowseButton.Name = "FolderBrowseButton";
			this.FolderBrowseButton.Size = new System.Drawing.Size(31, 20);
			this.FolderBrowseButton.TabIndex = 2;
			this.FolderBrowseButton.Text = "...";
			this.FolderBrowseButton.UseVisualStyleBackColor = true;
			this.FolderBrowseButton.Click += new System.EventHandler(FolderBrowseButton_Click);
			this.FolderBrowseDialog.Description = "Choose Folder";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.flowLayoutPanel1);
			base.Name = "FolderSelectionControl";
			base.Size = new System.Drawing.Size(266, 26);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
